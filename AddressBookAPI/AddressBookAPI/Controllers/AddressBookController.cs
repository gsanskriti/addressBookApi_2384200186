using BusinessLayer.Interface;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.model;

namespace AddressBookAPI.Controllers
{
    [Route("api/addressbook")]
    [ApiController]
    public class AddressBookController : ControllerBase
    {
        private readonly IAddressBL _addressBL; // Business Layer Interface for AddressBook

        public AddressBookController(IAddressBL addressBL)
        {
            _addressBL = addressBL; // Injecting Business Layer into Controller
        }

        /// <summary>
        /// Fetch all contacts from the address book.
        /// </summary>
        /// <returns>Returns a list of all contacts.</returns>
        [HttpGet]
        public IActionResult GetAllContacts()
        {
            var contacts = _addressBL.GetAllContactsAsync(); // Fetch contacts from Business Layer
            if (contacts == null)
                return NotFound(new { message = "No contacts found" });

            return Ok(new { message = "Contacts retrieved successfully", data = contacts });
        }

        /// <summary>
        /// Fetch a specific contact by ID.
        /// </summary>
        /// <param name="id">ID of the contact to retrieve.</param>
        /// <returns>Returns the requested contact.</returns>
        [HttpGet("get/{id}")]
        public IActionResult GetContactById(int id)
        {
            var contact = _addressBL.GetContactByIdAsync(id); // Fetch contact by ID from Business Layer
            if (contact == null)
                return NotFound(new { message = $"Contact with ID {id} not found" });

            return Ok(new { message = "Contact retrieved successfully", data = contact });
        }

        /// <summary>
        /// Add a new contact to the address book.
        /// </summary>
        /// <param name="request">Request model containing contact details.</param>
        /// <returns>Returns the newly added contact.</returns>
        [HttpPost("add")]
        public IActionResult AddContact([FromBody] RequestModel request)
        {
            if (request == null)
                return BadRequest(new { message = "Invalid request data" });

            var newContact = _addressBL.AddContactAsync(request); // Add contact via Business Layer
            if (newContact == null)
                return BadRequest(new { message = "Failed to add contact" });

            return CreatedAtAction(nameof(GetContactById), new { id = newContact.Id },
                new { message = "Contact added successfully", data = newContact });
        }

        /// <summary>
        /// Update an existing contact.
        /// </summary>
        /// <param name="id">ID of the contact to update.</param>
        /// <param name="request">Updated contact details.</param>
        /// <returns>Returns the updated contact.</returns>
        [HttpPut("update/{id}")]
        public IActionResult UpdateContact(int id, [FromBody] RequestModel request)
        {
            var updatedContact = _addressBL.UpdateContactAsync(id, request); // Update contact via Business Layer
            if (updatedContact == null)
                return NotFound(new { message = $"Contact with ID {id} not found" });

            return Ok(new { message = "Contact updated successfully", data = updatedContact });
        }

        /// <summary>
        /// Delete a contact from the address book.
        /// </summary>
        /// <param name="id">ID of the contact to delete.</param>
        /// <returns>Returns success message if deleted.</returns>
        [HttpDelete("delete/{id}")]
        public IActionResult DeleteContact(int id)
        {
            var isDeleted = _addressBL.DeleteContactAsync(id); // Delete contact via Business Layer
            if (isDeleted == null)
                return NotFound(new { message = $"Contact with ID {id} not found" });

            return Ok(new { message = "Contact deleted successfully" });
        }
    }
}
