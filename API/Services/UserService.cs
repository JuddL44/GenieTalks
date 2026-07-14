using System.Net.Mail;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public class UserService : IUserService
{
    private readonly AppDbContext _context;

    public UserService(AppDbContext context)
    {
        _context = context;
    }


    public async Task<ServiceResult<UserResponse>> CreateUserAsync(CreateUserRequest userReq)
    {
        //
        // Is the password valid?
        //
        if (userReq.Password.Length <= 6)
        {
            return new ServiceResult<UserResponse>
            (
                false,
                null,
                "Password must be longer than 6 characters!"
            );
        }
        if (userReq.Password.Length >= 26)
        {
            return new ServiceResult<UserResponse>
            (
                false,
                null,
                "Password must be shorter than 26 characters!"
            );
        }
        if (!userReq.Password.Any(char.IsUpper))
        {
            return new ServiceResult<UserResponse>
            (
                false,
                null,
                "Password must contain an uppercase letter!"
            );
        }
        if (!userReq.Password.Any(char.IsLower))
        {
            return new ServiceResult<UserResponse>
            (
                false,
                null,
                "Password must contain a lowercase letter!"
            );
        }
        if (!userReq.Password.Any(char.IsDigit))
        {
            return new ServiceResult<UserResponse>
            (
                false,
                null,
                "Password must contain atleast one number!"
            );
        }
        //
        // Is the email valid?
        //
        if (!IsValidEmailFormat(userReq.Email))
        {
            return new ServiceResult<UserResponse>
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
            return new ServiceResult<UserResponse>
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
        User user = new User
        {
            Email = userReq.Email,
            PasswordHash = _service.Hash(userReq.Password)
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        UserResponse response = new UserResponse
        {
            Email = user.Email
        };
        return new ServiceResult<UserResponse>(
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
}