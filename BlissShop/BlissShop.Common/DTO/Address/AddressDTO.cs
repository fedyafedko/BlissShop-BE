namespace BlissShop.Common.DTO.Address;

public class AddressDTO
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Country { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
}