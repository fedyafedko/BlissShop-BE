﻿namespace BlissShop.Common.DTO.User;

public class UserDTO
{
    public Guid Id { get; set;}
    public string FullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string UrlAvatar { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}
