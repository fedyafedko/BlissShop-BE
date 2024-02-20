namespace BlissShop.Common.DTO.Auth;

public class ConfirmEmailDTO
{
    public Guid UserId { get; set; }
    public int Code { get; set; }
}
