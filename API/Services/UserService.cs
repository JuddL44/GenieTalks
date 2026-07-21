using System.Net.Mail;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

public class UserService : IUserService
{
    private readonly AppDbContext _context;
    private readonly ITokenService _tokenService;
    private readonly JwtSettings _jwtSettings;

    public UserService(AppDbContext context, ITokenService tokenService, IOptions<JwtSettings> jwtSettings)
    {
        _context = context;
        _jwtSettings = jwtSettings.Value;
        _tokenService = tokenService;
    }


    public async Task<ServiceResult<AuthResponse>> CreateUserAsync(CreateUserRequest userReq)
    {
        //
        // Is the password valid?
        //
        string passwordStatus = IsValidPasswordFormat(userReq.Password);
        if (passwordStatus != "Success")
        {
            return new ServiceResult<AuthResponse>
            (
                false,
                null,
                passwordStatus
            );
        }
        //
        // Is the email valid?
        //
        if (!IsValidEmailFormat(userReq.Email))
        {
            return new ServiceResult<AuthResponse>
            (
                false,
                null,
                "Please enter a valid email address!"
            );
        }
        //
        // Does the email already exist?
        //
        var existingEmail = await _context.Users.AnyAsync(e => e.Email == userReq.Email); 
        if (existingEmail)
        {
            return new ServiceResult<AuthResponse>
            (
                false,
                null,
                "An account with that email already exists!"
            );
        }
        // 
        // Creating the user
        //
        PasswordService _service = new PasswordService(); 
        User newUser = new User
        {
            Email = userReq.Email,
            PasswordHash = _service.Hash(userReq.Password)
        };
        CreditWallet wallet = new CreditWallet
        {
            UserId = newUser.Id,
            user = newUser,
            Balance = 0
        };
        newUser.CreditWallet = wallet;
        _context.Users.Add(newUser);
        await _context.SaveChangesAsync();

        var token = _tokenService.CreateToken(newUser);

        AuthResponse response = new AuthResponse
        {
            Token = token,
            ExpiresAtUtc = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes),
            User = new UserResponse
            {
                Email = newUser.Email
            }
        };

        return new ServiceResult<AuthResponse>(
            true,
            response,
            "Success!"
        );
    }

    public async Task<ServiceResult<UserResponse>> GetUserByIdAsync(Guid id)
    {
        var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
        if (user != null)
        {
            var response = new UserResponse
            {
                Email = user.Email,
            };
            return new ServiceResult<UserResponse>
            (
                true,
                response,
                "Successfully found user!"
            );
        }
        else
        {
            return new ServiceResult<UserResponse>
            (
                false,
                null,
                $"Could not find user with ID: {id}"
            );
        }
    }

    public async Task<ServiceResult<UserResponse>> UpdateUserByIdAsync(UpdateUserRequest update, Guid id)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (user == null)
        {
            return new ServiceResult<UserResponse>
            (
                false,
                null,
                $"Could not find user with ID: {id}"
            );
        }
        if (update.Email != null)
        {
            if (IsValidEmailFormat(update.Email))
            {
                user.Email = update.Email;
            }
            else
            {
                return new ServiceResult<UserResponse>
                (
                    false,
                    null,
                    "Please enter a valid email address!"
                );           
            }
        }
        if (update.Password != null)
        {
            string passwordStatus = IsValidPasswordFormat(update.Password);
            if (passwordStatus == "Success")
            {
                PasswordService _service = new PasswordService(); 
                user.PasswordHash = _service.Hash(update.Password);
            }
            else
            {
                return new ServiceResult<UserResponse>
                (
                    false,
                    null,
                    passwordStatus
                );
            }
        }
        var result = new UserResponse
        {
            Email = user.Email,
        };
        await _context.SaveChangesAsync();
        return new ServiceResult<UserResponse>
        (
            true,
            result,
            "Successfully updated user!"
        );
    }

    public async Task<ServiceResult<UserResponse>> DeleteUserByIdAsync(Guid id)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (user == null)
        {
            return new ServiceResult<UserResponse>
            (
                false,
                null,
                $"Could not find user with ID: {id}"
            );
        }
        _context.Users.Remove(user);
        return new ServiceResult<UserResponse>
        (
            true,
            null,
            "Successfully deleted user."
        );
    }

    public async Task<ServiceResult<AuthResponse>> LoginUserAsync(UserLoginRequest userReq)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userReq.Email);
        if (user == null)
        {
            return new ServiceResult<AuthResponse>
            (
                false,
                null,
                "Invalid email or password."
            );
        }
        PasswordService _service = new PasswordService(); 
        var passwordResult = _service.Verify(userReq.Password, user.PasswordHash);
        if (passwordResult == false)
        {
            return new ServiceResult<AuthResponse>
            (
                false,
                null,
                "Invalid email or password."
            );
        }

        var token = _tokenService.CreateToken(user);

        var response = new AuthResponse
        {
            Token = token,
            ExpiresAtUtc = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes),
            User = new UserResponse
            {
                Email = user.Email
            }
        };

        return new ServiceResult<AuthResponse>
        (
            true,
            response,
            "Success!"
        );
    }
    bool IsValidEmailFormat(string email)
    {
        if (string.IsNullOrWhiteSpace(email)) {return false;}
        try
        {
            var address = new MailAddress(email.Trim());
            int atIndex = address.Address.LastIndexOf('@');
            string domain = address.Address[(atIndex + 1)..];

            return address.Address == email.Trim() && domain.Contains('.') && !domain.EndsWith('.');
        }
        catch(FormatException)
        {
            return false;
        }
    }
    string IsValidPasswordFormat(string password)
    {
        if (password.Length <= 6) return "Password must be longer than 6 characters!";
        if (password.Length >= 26) return "Password must be shorter than 26 characters!";
        if (!password.Any(char.IsUpper)) return "Password must contain an uppercase letter!";
        if (!password.Any(char.IsLower)) return "Password must contain a lowercase letter!";
        if (!password.Any(char.IsDigit)) return "Password must contain atleast one number!";
        return "Success";
    }


}

