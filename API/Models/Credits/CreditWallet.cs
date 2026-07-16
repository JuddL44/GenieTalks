using Microsoft.Identity.Client;
using Microsoft.Net.Http.Headers;

public class CreditWallet
{
    public Guid Id {get; set;}
    public Guid UserId {get; set;}
    public User user {get; set;} = null!;
    public int Balance {get; set;}
}