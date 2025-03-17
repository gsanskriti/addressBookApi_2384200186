using ModelLayer.model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RepositoryLayer.Interface
{
    public interface IAddressRL
    {
        Task<IEnumerable<ResponseModel>> GetAllContactsAsync();
        Task<ResponseModel?> GetContactByIdAsync(int id);
        Task<ResponseModel> AddContactAsync(RequestModel model);
        Task<ResponseModel?> UpdateContactAsync(int id, RequestModel model);
        Task<bool> DeleteContactAsync(int id);
    }
}
