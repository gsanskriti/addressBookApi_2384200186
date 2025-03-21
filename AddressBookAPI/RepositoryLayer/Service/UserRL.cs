﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RepositoryLayer.Context;
using RepositoryLayer.Entity;
using RepositoryLayer.Interface;
using ModelLayer.DTOs;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BCrypt.Net;
using System.Linq;
using System.Net.Mail;
using System.Net;
using StackExchange.Redis;
using AddressBookAPI.RepositoryLayer.Interface;

namespace RepositoryLayer.Service
{
    public class UserRL : IUserRL
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IRedisCacheService _redisCacheService;
        private readonly IRabbitMQService _rabbitMQService;



        public UserRL(AppDbContext context, IConfiguration configuration, IRedisCacheService redisCacheService, IRabbitMQService rabbitMQService)
        {
            _context = context;
            _configuration = configuration;
            _redisCacheService = redisCacheService;
            _rabbitMQService = rabbitMQService;
        }

        //Register User & Invalidate Cache
        public async Task<User> Register(UserRegisterDTO userDto)
        {
            try
            {
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(userDto.Password);

                var user = new User
                {
                    FullName = userDto.FullName,
                    Email = userDto.Email,
                    PasswordHash = hashedPassword
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                await _redisCacheService.RemoveCacheValueAsync("AddressBook_Users");

                string message = $"New user registered: {user.Email}";
                _rabbitMQService.PublishMessage("UserRegisteredQueue", message);

                return user;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error: {ex.InnerException?.Message ?? ex.Message}");
            }
        }


        //Authenticate User
        public async Task<User> Authenticate(UserLoginDTO userDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userDto.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(userDto.Password, user.PasswordHash))
                throw new UnauthorizedAccessException("Invalid email or password");

            return user;
        }

        //Generate JWT Token
        public string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                    new Claim(ClaimTypes.Email, user.Email)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        //Forgot Password
        public async Task<bool> ForgotPasswordAsync(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null) return false;

            // Generate reset token
            user.ResetToken = Guid.NewGuid().ToString();
            user.ResetTokenExpires = DateTime.UtcNow.AddHours(1);

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            // Send Email
            await SendEmailAsync(email, "Password Reset", $"Your reset token: {user.ResetToken}");

            // Invalidate cache
            await _redisCacheService.RemoveCacheValueAsync("AddressBook_Users");

            return true;
        }

        //Reset Password
        public async Task<bool> ResetPasswordAsync(string email, string token, string newPassword)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email && u.ResetToken == token);
            if (user == null || user.ResetTokenExpires < DateTime.UtcNow)
                return false;

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            user.ResetToken = null;
            user.ResetTokenExpires = null;

            _context.Users.Update(user);
            bool isUpdated = await _context.SaveChangesAsync() > 0;

            // Invalidate cache
            await _redisCacheService.RemoveCacheValueAsync("AddressBook_Users");

            return isUpdated;
        }

        // Send Email Utility
        private async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var smtpSettings = _configuration.GetSection("EmailSettings");

            using var smtpClient = new SmtpClient(smtpSettings["SmtpServer"], int.Parse(smtpSettings["SmtpPort"]))
            {
                Credentials = new NetworkCredential(smtpSettings["SmtpUser"], smtpSettings["SmtpPass"]),
                EnableSsl = true,
                UseDefaultCredentials = false // Ensure only provided credentials are used
            };

            using var message = new MailMessage
            {
                From = new MailAddress(smtpSettings["FromEmail"]),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            message.To.Add(toEmail);

            try
            {
                await smtpClient.SendMailAsync(message);
            }
            catch (SmtpException ex)
            {
                throw new Exception($"SMTP Error: {ex.Message}");
            }
        }

        // Store user session in Redis
        public async Task StoreUserSession(string userId, string token)
        {
            await _redisCacheService.SetCacheValueAsync($"session_{userId}", token, TimeSpan.FromHours(1));
        }

        // Get user session from Redis
        public async Task<string> GetUserSession(string userId)
        {
            return await _redisCacheService.GetCacheValueAsync<string>($"session_{userId}");
        }

        // Get All Users (Cache Enabled)
        public async Task<List<User>> GetAllUsers()
        {
            string cacheKey = "AddressBook_Users";

            // Check Redis cache
            var cachedUsers = await _redisCacheService.GetCacheValueAsync<List<User>>(cacheKey);
            if (cachedUsers != null) return cachedUsers;

            // Fetch from DB if not in Redis
            var users = await _context.Users.ToListAsync();

            // Store in Redis cache for 30 minutes
            await _redisCacheService.SetCacheValueAsync(cacheKey, users, TimeSpan.FromMinutes(30));

            return users;
        }



    }
}
