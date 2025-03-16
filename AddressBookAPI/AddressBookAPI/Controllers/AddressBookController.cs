using Microsoft.AspNetCore.Mvc;
using ModelLayer.model;

namespace AddressBookAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class AddressBookController : ControllerBase
{
    private static List<ResponseModel> _contacts = new List<ResponseModel>();
    private static int _nextId = 1;

    
    /// <summary>
    /// GET: Fetch all contacts
    /// </summary>
    [HttpGet]
    public ActionResult<IEnumerable<ResponseModel>> GetAllContacts()
    {
        if (_contacts.Count == 0)
            return NotFound(new { message = "No contacts found" });

        return Ok(new { message = "Contacts retrieved successfully", data = _contacts });
    }

    /// <summary>
    /// Fetch contact by ID
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("get/{id}")]
    public ActionResult<ResponseModel> GetContactById(int id)
    {
        var contact = _contacts.Find(c => c.Id == id);
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
    public ActionResult<ResponseModel> AddContact([FromBody] ResponseModel request)
    {
        if (request == null)
            return BadRequest(new { message = "Invalid request data" });

        var newContact = new ResponseModel
        {
            Id = _nextId++,
            Name = request.Name,
            PhoneNumber = request.PhoneNumber,
            Email = request.Email,
            Address = request.Address
        };

        _contacts.Add(newContact);
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
    public ActionResult<ResponseModel> UpdateContact(int id, [FromBody] ResponseModel request)
    {
        var contact = _contacts.Find(c => c.Id == id);
        if (contact == null)
            return NotFound(new { message = $"Contact with ID {id} not found" });

        contact.Name = request.Name;
        contact.PhoneNumber = request.PhoneNumber;
        contact.Email = request.Email;
        contact.Address = request.Address;

        return Ok(new { message = "Contact updated successfully", data = contact });
    }

    /// <summary>
    /// Delete contact
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("delete/{id}")]
    public ActionResult DeleteContact(int id)
    {
        var contact = _contacts.Find(c => c.Id == id);
        if (contact == null)
            return NotFound(new { message = $"Contact with ID {id} not found" });

        _contacts.Remove(contact);
        return Ok(new { message = "Contact deleted successfully" });
    }

}

