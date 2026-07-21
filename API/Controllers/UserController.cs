using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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


    [HttpPost("register")]
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

    [HttpPost("login")]
    public async Task<IActionResult> LoginUser(UserLoginRequest userReq)
    {
        var result = await _userService.LoginUserAsync(userReq);
        if (!result.Success)
        {
            return Unauthorized(result.Log);
        }
        return Ok(result.Data);
    }


    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetUser()
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdValue, out var userId)) { return Unauthorized(); }
        var result = await _userService.GetUserByIdAsync(userId);
        if (result.Success)
        {
            return Ok(result.Data);
        }
        else
        {
            return BadRequest(new {message = result.Log});
        }

    }

    [Authorize]
    [HttpPatch]
    public async Task<IActionResult> UpdateUser(UpdateUserRequest update)
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdValue, out var userId)) { return Unauthorized(); }
        var result = await _userService.UpdateUserByIdAsync(update, userId);
        if (result.Success)
        {
            return Ok(result.Data);
        }
        else
        {
            return BadRequest(new {message = result.Log});
        }
    }


    [Authorize]
    [HttpDelete]
    public async Task<IActionResult> DeleteUser()
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdValue, out var userId)) { return Unauthorized(); }
        var result = await _userService.DeleteUserByIdAsync(userId);
        if (result.Success)
        {
            return Ok(result.Data);
        }
        else
        {
            return BadRequest(new {message = result.Log});
        }
    }
}