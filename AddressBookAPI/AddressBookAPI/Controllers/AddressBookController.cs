using Microsoft.AspNetCore.Mvc;

namespace AddressBookAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class AddressBookController : ControllerBase
{
    [HttpGet]
    public IActionResult GetEntries()
    {
        return Ok(new { message = "API is working!" });
    }

}

