using BusinessLayer.Interface;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AddressBookAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AddressBookController : ControllerBase
    {
        private readonly IAddressBL _addressBL;

        public AddressBookController(IAddressBL addressBL)
        {
            _addressBL = addressBL;
        }

        /// <summary>
        /// GET: Fetch all contacts
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ResponseModel>>> GetAllContacts()
        {
            var contacts = await _addressBL.GetAllContactsAsync();
            if (contacts == null || !contacts.Any())
                return NotFound(new { message = "No contacts found" });

            return Ok(new { message = "Contacts retrieved successfully", data = contacts });
        }

        /// <summary>
        /// Fetch contact by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("get/{id}")]
        public async Task<ActionResult<ResponseModel>> GetContactById(int id)
        {
            var contact = await _addressBL.GetContactByIdAsync(id);
            if (contact == null)
                return NotFound(new { message = $"Contact with ID {id} not found" });

            return Ok(new { message = "Contact retrieved successfully", data = contact });
        }

        /// <summary>
        /// Add new contact
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("add")]
        public async Task<ActionResult<ResponseModel>> AddContact([FromBody] RequestModel request)
        {
            if (request == null)
                return BadRequest(new { message = "Invalid request data" });

            var newContact = await _addressBL.AddContactAsync(request);
            return CreatedAtAction(nameof(GetContactById), new { id = newContact.Id },
                new { message = "Contact added successfully", data = newContact });
        }

        /// <summary>
        /// Update contact
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut("update/{id}")]
        public async Task<ActionResult<ResponseModel>> UpdateContact(int id, [FromBody] RequestModel request)
        {
            var updatedContact = await _addressBL.UpdateContactAsync(id, request);
            if (updatedContact == null)
                return NotFound(new { message = $"Contact with ID {id} not found" });

            return Ok(new { message = "Contact updated successfully", data = updatedContact });
        }

        /// <summary>
        /// Delete contact
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("delete/{id}")]
        public async Task<ActionResult> DeleteContact(int id)
        {
            var isDeleted = await _addressBL.DeleteContactAsync(id);
            if (!isDeleted)
                return NotFound(new { message = $"Contact with ID {id} not found" });

            return Ok(new { message = "Contact deleted successfully" });
        }
    }
}
