using ModelLayer.DTOs;
using RepositoryLayer.Entity;

namespace BusinessLayer.Interface
{
    public interface IUserBL
    {
        Task<UserDTO> RegisterUser(UserRegisterDTO userDto);
        Task<string?> LoginUser(UserLoginDTO userDto);
        public Task<bool> ForgotPassword(string email);
        public Task<bool> ResetPassword(string email, string token, string newPassword);

    }
}
