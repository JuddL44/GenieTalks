using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class UserController: ControllerBase
{
    private readonly AppDbContext _context;
    private readonly UserService _userService;
    public UserController(AppDbContext context, UserService userService)
    {
        _context = context;
        _userService = userService;
    }


    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(Guid id)
    {
        var result = await _userService.GetUserByIdAsync(id);
        if (result.Success == true)
        {
            return Ok(result.Data);
        }
        else
        {
            return BadRequest(new {message = result.Log});
        }

    }

    [HttpPatch("{id}")]
    public IActionResult UpdateUserById(Guid id)
    {
        throw new NotImplementedException();
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser(CreateUserRequest user)
    {
        var result = await _userService.CreateUserAsync(user);
        if (result.Success == true)
        {
            return Ok(result.Data);
        }
        else
        {
            return BadRequest(new {message = result.Log});
        }
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteUserById(Guid id)
    {
        throw new NotImplementedException();
    }
}