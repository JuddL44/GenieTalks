public sealed class AuthResponse
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAtUtc {get; set;}
    public UserResponse User {get; set;} = null!;
}