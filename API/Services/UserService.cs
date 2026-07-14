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
        if (!userReq.Password.Any(char.IsUpper) || !userReq.Password.Any(char.IsLower) || !userReq.Password.Any(char.IsDigit))
        {
            return new ServiceResult<UserResponse>
            (
                false,
                null,
                "Passwords must contain an uppercase letter, a lowercase letter, and a number!"
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