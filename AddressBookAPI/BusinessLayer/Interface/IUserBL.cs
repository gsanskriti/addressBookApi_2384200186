using ModelLayer.DTOs;
using RepositoryLayer.Entity;

namespace BusinessLayer.Interface
{
    public interface IUserBL
    {
        Task<UserDTO> RegisterUser(UserRegisterDTO userDto);
        Task<string?> LoginUser(UserLoginDTO userDto);
    }
}
