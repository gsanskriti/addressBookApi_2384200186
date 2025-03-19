using AddressBookAPI.BusinessLayer.Interface;
using AddressBookAPI.Models.DTOs;
using ModelLayer.DTOs;
using RepositoryLayer.Interface;
using System.Threading.Tasks;

namespace AddressBookAPI.BusinessLayer.Service
{
    public class AuthService : IAuthService
    {
        private readonly IUserRL _userRepository;

        public AuthService(IUserRL userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<bool> RegisterAsync(UserRegisterDTO model)
        {
            var user = await _userRepository.Register(new UserRegisterDTO
            {
                FullName = model.FullName,
                Email = model.Email,
                Password = model.Password
            });

            return user != null;
        }

        public async Task<string?> LoginAsync(UserLoginDTO model)
        {
            var user = await _userRepository.Authenticate(new UserLoginDTO
            {
                Email = model.Email,
                Password = model.Password
            });

            return user != null ? _userRepository.GenerateJwtToken(user) : null;
        }

        public async Task<bool> ForgotPasswordAsync(ForgotPasswordDto model)
        {
            return await _userRepository.ForgotPasswordAsync(model.Email);
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordDto model)
        {
            return await _userRepository.ResetPasswordAsync(model.Email, model.Token, model.NewPassword);
        }
    }
}
