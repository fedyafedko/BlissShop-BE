﻿namespace BlissShop.Common.DTO.Shop;

public class ShopDTO
{
    public Guid Id { get; set; }
    public Guid SellerId { get; set; }
    public string Name { get; set; } = string.Empty;
}