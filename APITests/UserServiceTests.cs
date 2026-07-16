using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

public class UserServiceTests
{
    private static AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        return new AppDbContext(options);
    }


    [Fact]
    public async Task CreateUserAsync_WithDuplicateEmail_ReturnsFailure()
    {
        await using var context = CreateDbContext();
        context.Users.Add(new User
        {
            Email = "JohnDoe@gmail.com",
            PasswordHash = "john-doe-password-hash"
        });
        await context.SaveChangesAsync();
        var service = new UserService(context);

        var request = new CreateUserRequest
        {
            Email = "JohnDoe@gmail.com",
            Password = "Password123!"
        };
        var result = await service.CreateUserAsync(request);

        Assert.False(result.Success);
        Assert.Equal("An account with that email already exists!", result.Log);
        Assert.Equal(1, await context.Users.CountAsync());
    }
    [Fact]
    public async Task CreateUserAsync_ContainsInvalidEmail_ReturnsFailure()
    {
        await using var context = CreateDbContext();
        var service = new UserService(context);
        var request = new CreateUserRequest
        {
            Email = "JohnDoe@gmailcom",
            Password = "Password123!"
        };
        var result = await service.CreateUserAsync(request);
        Assert.False(result.Success);
        Assert.Equal("Please enter a valid email address!", result.Log);
    }
    [Fact]
    public async Task CreateUserAsync_NoUppercaseWithinPassword_ReturnsFailure()
    {
        await using var context = CreateDbContext();
        var service = new UserService(context);
        var request = new CreateUserRequest
        {
            Email = "JohnDoe@gmail.com",
            Password="password123"
        };
        var result = await service.CreateUserAsync(request);
        Assert.False(result.Success);
        Assert.Equal("Password must contain an uppercase letter!", result.Log);
    }
    [Fact]
    public async Task CreateUserAsync_NoLowercaseithinPassword_ReturnsFailure()
    {
        await using var context = CreateDbContext();
        var service = new UserService(context);
        var request = new CreateUserRequest
        {
            Email = "JohnDoe@gmail.com",
            Password="PASSWORD123"
        };
        var result = await service.CreateUserAsync(request);
        Assert.False(result.Success);
        Assert.Equal("Password must contain a lowercase letter!", result.Log);
    }
    [Fact]
    public async Task CreateUserAsync_NoNumberWithinPassword_ReturnsFailure()
    {
        await using var context = CreateDbContext();
        var service = new UserService(context);
        var request = new CreateUserRequest
        {
            Email = "JohnDoe@gmail.com",
            Password="Password"
        };
        var result = await service.CreateUserAsync(request);
        Assert.False(result.Success);
        Assert.Equal("Password must contain atleast one number!", result.Log);
    }
    [Fact]
    public async Task CreateUserAsync_PasswordLessThan7Characters_ReturnsFailure()
    {
        await using var context = CreateDbContext();
        var service = new UserService(context);
        var request = new CreateUserRequest
        {
            Email = "JohnDoe@gmail.com",
            Password="Psw82"
        };
        var result = await service.CreateUserAsync(request);
        Assert.False(result.Success);
        Assert.Equal("Password must be longer than 6 characters!", result.Log);
    }
    [Fact]
    public async Task CreateUserAsync_PasswordMoreThan26Characters_ReturnsFailure()
    {
        await using var context = CreateDbContext();
        var service = new UserService(context);
        var request = new CreateUserRequest
        {
            Email = "JohnDoe@gmail.com",
            Password="VeryLongPassword123456789Passwords1234567Password"
        };
        var result = await service.CreateUserAsync(request);
        Assert.False(result.Success);
        Assert.Equal("Password must be shorter than 26 characters!", result.Log);
    }

    [Fact]
    public async Task CreateUserAsync_EverythingValid_ReturnsSuccess()
    {
        await using var context = CreateDbContext();
        var service = new UserService(context);
        var request = new CreateUserRequest
        {
            Email = "JaneDoe@hotmail.com",
            Password = "SecurePassword123"
        };
        var result = await service.CreateUserAsync(request);
        Assert.True(result.Success);
        Assert.Equal("Success!", result.Log);
        Assert.Equal("JaneDoe@hotmail.com", result.Data?.Email);
        Assert.Equal("JaneDoe", result.Data?.ShortenedEmail);
        Assert.Equal(1, await context.Users.CountAsync());
    }









    [Fact]
    public async Task GetUserByIdAsync_ValidId_ReturnsSuccess()
    {
        await using var context = CreateDbContext();
        var user = new User
        {
            Email = "JohnDoe@gmail.com",
            PasswordHash ="john-doe-password-hash"
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var service = new UserService(context);
        var result = await service.GetUserByIdAsync(user.Id);

        Assert.True(result.Success);
        Assert.Equal("Successfully found user!", result.Log);
        Assert.Equal("JohnDoe", result.Data?.ShortenedEmail);
    }

    [Fact]
    public async Task GetUserByIdAsync_InvalidId_ReturnsFailure()
    {
        await using var context = CreateDbContext();
        Guid fakeId = new Guid();
        var service = new UserService(context);
        var result = await service.GetUserByIdAsync(fakeId);
        Assert.False(result.Success);
        Assert.Equal($"Could not find user with ID: {fakeId}", result.Log);
    }

    [Fact]
    public async Task UpdateUserByIdAsync_InvalidId_ReturnsFailure()
    {
        await using var context = CreateDbContext();
        Guid fakeId = new Guid();
        var service = new UserService(context);
        UpdateUserRequest request = new UpdateUserRequest
        {
            Email = null,
            Password = "NewPassword123!"
        };
        var result = await service.UpdateUserByIdAsync(request, fakeId);
        Assert.False(result.Success);
        Assert.Equal($"Could not find user with ID: {fakeId}", result.Log);
    }

    [Fact]
    public async Task UpdateUserByIdAsync_InvalidEmail_ReturnsFailure()
    {
        await using var context = CreateDbContext();
        var user = new User
        {
            Email = "JohnDoe@gmail.com",
            PasswordHash = "john-doe-password-hash"
        };
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();
        var service = new UserService(context);
        UpdateUserRequest request = new UpdateUserRequest
        {
            Email = "JohnDoe@gmailcom",
            Password = null
        };
        var result = await service.UpdateUserByIdAsync(request, user.Id);
        Assert.False(result.Success);
        Assert.Equal("Please enter a valid email address!", result.Log);
    }

    [Fact]
    public async Task UpdateUserByIdAsync_EverythingValid_ReturnsSuccess()
    {
        await using var context = CreateDbContext();
        var user = new User
        {
            Email = "JohnDoe@gmail.com",
            PasswordHash = "john-doe-password-hash"
        };
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();
        var service = new UserService(context);
        UpdateUserRequest request = new UpdateUserRequest
        {
            Email = "JaneDoe@hotmail.com",
            Password = "NewPassword1234!2"
        };
        var result = await service.UpdateUserByIdAsync(request, user.Id);
        Assert.True(result.Success);
        Assert.Equal("Successfully updated user!", result.Log);
    }

    [Fact]
    public async Task UpdateUserByIdAsync_PasswordTooShort_ReturnsFailure()
    {
        await using var context = CreateDbContext();
        var user = new User
        {
            Email = "JohnDoe@gmail.com",
            PasswordHash = "john-doe-password-hash"
        };
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();
        var service = new UserService(context);
        UpdateUserRequest request = new UpdateUserRequest
        {
            Email = "JaneDoe@hotmail.com",
            Password = "Pas3!"
        };
        var result = await service.UpdateUserByIdAsync(request, user.Id);
        Assert.False(result.Success);
        Assert.Equal("Password must be longer than 6 characters!", result.Log);
    }

    [Fact]
    public async Task UpdateUserByIdAsync_PasswordTooLong_ReturnsFailure()
    {
        await using var context = CreateDbContext();
        var user = new User
        {
            Email = "JohnDoe@gmail.com",
            PasswordHash = "john-doe-password-hash"
        };
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();
        var service = new UserService(context);
        UpdateUserRequest request = new UpdateUserRequest
        {
            Email = "JaneDoe@hotmail.com",
            Password = "JaneDoesVeryLongUpdatedPassword12345678910!"
        };
        var result = await service.UpdateUserByIdAsync(request, user.Id);
        Assert.False(result.Success);
        Assert.Equal("Password must be shorter than 26 characters!", result.Log);
    }

    [Fact]
    public async Task UpdateUserByIdAsync_PasswordNoUppercaseLetter_ReturnsFailure()
    {
        await using var context = CreateDbContext();
        var user = new User
        {
            Email = "JohnDoe@gmail.com",
            PasswordHash = "john-doe-password-hash"
        };
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();
        var service = new UserService(context);
        UpdateUserRequest request = new UpdateUserRequest
        {
            Email = "JaneDoe@hotmail.com",
            Password = "password1234!"
        };
        var result = await service.UpdateUserByIdAsync(request, user.Id);
        Assert.False(result.Success);
        Assert.Equal("Password must contain an uppercase letter!", result.Log);
    }
    [Fact]
    public async Task UpdateUserByIdAsync_PasswordNoLowercaseLetter_ReturnsFailure()
    {
        await using var context = CreateDbContext();
        var user = new User
        {
            Email = "JohnDoe@gmail.com",
            PasswordHash = "john-doe-password-hash"
        };
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();
        var service = new UserService(context);
        UpdateUserRequest request = new UpdateUserRequest
        {
            Email = "JaneDoe@hotmail.com",
            Password = "PASSWORD6789?"
        };
        var result = await service.UpdateUserByIdAsync(request, user.Id);
        Assert.False(result.Success);
        Assert.Equal("Password must contain a lowercase letter!", result.Log);
    }
    [Fact]
    public async Task UpdateUserByIdAsync_PasswordDoesntContainNumber_ReturnsFailure()
    {
        await using var context = CreateDbContext();
        var user = new User
        {
            Email = "JohnDoe@gmail.com",
            PasswordHash = "john-doe-password-hash"
        };
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();
        var service = new UserService(context);
        UpdateUserRequest request = new UpdateUserRequest
        {
            Email = "JaneDoe@hotmail.com",
            Password = "NewPasswordNew!"
        };
        var result = await service.UpdateUserByIdAsync(request, user.Id);
        Assert.False(result.Success);
        Assert.Equal("Password must contain atleast one number!", result.Log);
    }

    [Fact]
    public async Task DeleteUserByIdAsync_InvalidId_ReturnsFailure()
    {
        await using var context = CreateDbContext();
        Guid fakeId = new Guid();
        var service = new UserService(context);
        var result = await service.DeleteUserByIdAsync(fakeId);
        Assert.False(result.Success);
        Assert.Equal($"Could not find user with ID: {fakeId}", result.Log);
    }

    [Fact]
    public async Task DeleteUserByIdAsync_ValidId_ReturnsSuccess()
    {
        await using var context = CreateDbContext();
        var user = new User
        {
            Email = "JohnDoe@gmail.com",
            PasswordHash = "john-doe-password-hash"
        };
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();
        var service = new UserService(context);
        var result = await service.DeleteUserByIdAsync(user.Id);
        Assert.True(result.Success);
        Assert.Equal("Successfully deleted user.", result.Log);
    }


    [Fact]
    public async Task CreateNewUser_AddsCreditWallet_ReturnsSuccess()
    {
        await using var context = CreateDbContext();
        var service = new UserService(context);
        var request = new CreateUserRequest
        {
            Email = "JaneDoe@hotmail.com",
            Password = "SecurePassword123"
        };
        await service.CreateUserAsync(request);
        var user = await context.Users.FirstOrDefaultAsync(u => u.Email == "JaneDoe@hotmail.com");
        Assert.NotNull(user);
        var wallet = await context.Wallets.FirstOrDefaultAsync(w => w.UserId == user.Id);
        Assert.NotNull(wallet);
        Assert.Equal(user.Id, wallet.UserId);
    }
}