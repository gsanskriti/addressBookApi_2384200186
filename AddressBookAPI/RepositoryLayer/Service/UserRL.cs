using Microsoft.EntityFrameworkCore;
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

namespace RepositoryLayer.Service
{
    public class UserRL : IUserRL
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public UserRL(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

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
                return user;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error: {ex.InnerException?.Message ?? ex.Message}");
            }
        }


        public async Task<User> Authenticate(UserLoginDTO userDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userDto.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(userDto.Password, user.PasswordHash))
                throw new UnauthorizedAccessException("Invalid email or password");

            return user;
        }

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

            return true;
        }

        public async Task<bool> ResetPasswordAsync(string email, string token, string newPassword)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email && u.ResetToken == token);
            if (user == null || user.ResetTokenExpires < DateTime.UtcNow)
                return false;

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            user.ResetToken = null;
            user.ResetTokenExpires = null;

            _context.Users.Update(user);
            return await _context.SaveChangesAsync() > 0;
        }


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



    }
}
