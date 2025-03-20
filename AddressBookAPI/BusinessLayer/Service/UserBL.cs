using BusinessLayer.Interface;
using ModelLayer.DTOs;
using RepositoryLayer.Interface;
using System.Threading.Tasks;

namespace BusinessLayer.Service
{
    public class UserBL : IUserBL
    {
        private readonly IUserRL _userRL;

        public UserBL(IUserRL userRL)
        {
            _userRL = userRL;
        }

        public async Task<UserDTO> RegisterUser(UserRegisterDTO userDto)
        {
            var user = await _userRL.Register(userDto);
            return new UserDTO { FullName = user.FullName, Email = user.Email };
        }

        public async Task<string?> LoginUser(UserLoginDTO userDto)
        {
            var user = await _userRL.Authenticate(userDto);
            if (user == null) return null;

            return _userRL.GenerateJwtToken(user);
        }

        public async Task<bool> ForgotPassword(string email)
        {
            return await _userRL.ForgotPasswordAsync(email);
        }

        public async Task<bool> ResetPassword(string email, string token, string newPassword)
        {
            return await _userRL.ResetPasswordAsync(email, token, newPassword);
        }
    }
}
