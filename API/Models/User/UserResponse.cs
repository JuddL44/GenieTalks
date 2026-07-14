using Microsoft.Identity.Client;

public class UserResponse
{
    public string Email {get; set;} = string.Empty;
    public string ShortenedEmail
    {
        get
        {
            int index = Email.IndexOf('@');
            return index >= 0 ? Email[..index] : Email;
        }
    }
}