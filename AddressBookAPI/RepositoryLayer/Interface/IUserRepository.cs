using System.Threading.Tasks;
using RepositoryLayer.Entity;

namespace AddressBookAPI.RepositoryLayer.Interface
{
    public interface IUserRepository
    {
        Task<User> GetUserByEmailAsync(string email);
        Task<bool> UpdateUserAsync(User user);
    }
}
