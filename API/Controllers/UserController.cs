using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class UserController: ControllerBase
{
    private readonly UserService _service;
    public UserController(UserService service)
    {
        _service = service;
    }


    [HttpGet("{id}")]
    public IActionResult GetUserById(Guid id)
    {
        throw new NotImplementedException();
    }

    [HttpPatch("{id}")]
    public IActionResult UpdateUserById(Guid id)
    {
        throw new NotImplementedException();
    }

    [HttpPost]
    public IActionResult CreateUser(CreateUserRequest user)
    {
        throw new NotImplementedException();
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteUserById(Guid id)
    {
        throw new NotImplementedException();
    }
}