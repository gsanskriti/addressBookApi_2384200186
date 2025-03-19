using AddressBookAPI.Models.DTOs;
using System.Threading.Tasks;
using ModelLayer.DTOs;

namespace AddressBookAPI.BusinessLayer.Interface
{
    public interface IAuthService
    {
        Task<bool> RegisterAsync(UserRegisterDTO model);
        Task<string?> LoginAsync(UserLoginDTO model);
        Task<bool> ForgotPasswordAsync(ForgotPasswordDto model);
        Task<bool> ResetPasswordAsync(ResetPasswordDto model);
    }
}
