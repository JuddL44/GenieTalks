public class CreditPurchase
{
    public Guid Id {get; set;}
    public Guid WalletId {get; set;}
    public Guid UserId {get; set;}
    public int Amount {get; set;}
    public DateTime PurchasedAt {get; set;} = DateTime.UtcNow; 
}