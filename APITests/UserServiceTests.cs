public class UserServiceTests
{
    [Fact]
    public void Test1()
    {
    var serviceType = typeof(global::UserService);

    Assert.NotNull(serviceType);
    }
}