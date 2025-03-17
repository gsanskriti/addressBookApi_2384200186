using BusinessLayer.Interface;
using ModelLayer.model;
using RepositoryLayer.Interface;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLayer.Service
{
    public class AddressBL : IAddressBL
    {
        private readonly IAddressRL _addressRL;

        public AddressBL(IAddressRL addressRL)
        {
            _addressRL = addressRL;
        }

        public async Task<IEnumerable<ResponseModel>> GetAllContactsAsync()
        {
            return await _addressRL.GetAllContactsAsync();
        }

        public async Task<ResponseModel?> GetContactByIdAsync(int id)
        {
            return await _addressRL.GetContactByIdAsync(id);
        }

        public async Task<ResponseModel> AddContactAsync(RequestModel model)
        {
            return await _addressRL.AddContactAsync(model);
        }

        public async Task<ResponseModel?> UpdateContactAsync(int id, RequestModel model)
        {
            return await _addressRL.UpdateContactAsync(id, model);
        }

        public async Task<bool> DeleteContactAsync(int id)
        {
            return await _addressRL.DeleteContactAsync(id);
        }
    }
}
