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
        Assert.Equal("An account with that email already exists!", result.Error);
        Assert.Equal(1, await context.Users.CountAsync());

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
        Assert.Equal("Password must contain an uppercase letter!", result.Error);
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
        Assert.Equal("Password must contain a lowercase letter!", result.Error);
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
        Assert.Equal("Password must contain atleast one number!", result.Error);
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
        Assert.Equal("Password must be longer than 6 characters!", result.Error);
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
        Assert.Equal("Password must be shorter than 26 characters!", result.Error);
    }
}