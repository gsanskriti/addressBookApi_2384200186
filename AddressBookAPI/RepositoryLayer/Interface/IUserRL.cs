using ModelLayer.DTOs;
using RepositoryLayer.Entity;
using System.Threading.Tasks;

namespace RepositoryLayer.Interface
{
    public interface IUserRL
    {
        Task<User> Register(UserRegisterDTO userDto);
        Task<User> Authenticate(UserLoginDTO userDto);
        string GenerateJwtToken(User user);
        Task<bool> ForgotPasswordAsync(string email);
        Task<bool> ResetPasswordAsync(string email, string token, string newPassword);
    }
}
