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
}