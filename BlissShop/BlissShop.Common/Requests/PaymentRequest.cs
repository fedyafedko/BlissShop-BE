namespace BlissShop.Common.Requests;

public class PaymentRequest
{
    public Guid CartId { get; set; }
    public Guid AddressId { get; set; }
}
