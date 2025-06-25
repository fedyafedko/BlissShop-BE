namespace BlissShop.Common.Requests;

public class  ApprovedShopRequest
{
    public Guid ShopId { get; set; }
    public bool IsApproved { get; set; }
}