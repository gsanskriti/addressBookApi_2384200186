using Microsoft.EntityFrameworkCore;
using RepositoryLayer.Context;
using RepositoryLayer.Interface;
using RepositoryLayer.Entity;
using ModelLayer.model;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RepositoryLayer.Service
{
    public class AddressRL : IAddressRL
    {
        private readonly AppDbContext _context;

        public AddressRL(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ResponseModel>> GetAllContactsAsync()
        {
            return await _context.AddressBookEntries
                .Select(a => new ResponseModel
                {
                    Id = a.Id,
                    Name = a.Name,
                    PhoneNumber = a.PhoneNumber,
                    Email = a.Email,
                    Address = a.Address
                })
                .ToListAsync();
        }

        public async Task<ResponseModel?> GetContactByIdAsync(int id)
        {
            var contact = await _context.AddressBookEntries.FindAsync(id);
            if (contact == null)
                return null;

            return new ResponseModel
            {
                Id = contact.Id,
                Name = contact.Name,
                PhoneNumber = contact.PhoneNumber,
                Email = contact.Email,
                Address = contact.Address
            };
        }

        public async Task<ResponseModel> AddContactAsync(RequestModel model)
        {
            var newEntry = new AddressBookEntry
            {
                Name = model.Name!,
                PhoneNumber = model.PhoneNumber!,
                Email = model.Email!,
                Address = model.Address!,
                UserId = 1 // TODO: Replace with actual user ID handling
            };

            _context.AddressBookEntries.Add(newEntry);
            await _context.SaveChangesAsync();

            return new ResponseModel
            {
                Id = newEntry.Id,
                Name = newEntry.Name,
                PhoneNumber = newEntry.PhoneNumber,
                Email = newEntry.Email,
                Address = newEntry.Address
            };
        }

        public async Task<ResponseModel?> UpdateContactAsync(int id, RequestModel model)
        {
            var contact = await _context.AddressBookEntries.FindAsync(id);
            if (contact == null)
                return null;

            contact.Name = model.Name!;
            contact.PhoneNumber = model.PhoneNumber!;
            contact.Email = model.Email!;
            contact.Address = model.Address!;

            await _context.SaveChangesAsync();

            return new ResponseModel
            {
                Id = contact.Id,
                Name = contact.Name,
                PhoneNumber = contact.PhoneNumber,
                Email = contact.Email,
                Address = contact.Address
            };
        }

        public async Task<bool> DeleteContactAsync(int id)
        {
            var contact = await _context.AddressBookEntries.FindAsync(id);
            if (contact == null) return false;

            _context.AddressBookEntries.Remove(contact);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
