using ModelLayer.model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLayer.Interface
{
    public interface IAddressBL
    {
        Task<IEnumerable<ResponseModel>> GetAllContactsAsync();
        Task<ResponseModel?> GetContactByIdAsync(int id);
        Task<ResponseModel> AddContactAsync(RequestModel model);
        Task<ResponseModel?> UpdateContactAsync(int id, RequestModel model);
        Task<bool> DeleteContactAsync(int id);
    }
}
