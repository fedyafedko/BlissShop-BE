﻿using BlissShop.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BlissShop.DAL.EF;

public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
    public DbSet<Address> Addresses { get; set; } = null!;
    public DbSet<Order> Orders { get; set; } = null!;
    public DbSet<Product> Products { get; set; } = null!;
    public DbSet<Shop> Shops { get; set; } = null!;
    public DbSet<UserRegister> UserRegisters { get; set; } = null!;
    public DbSet<ProductCart> ProductCarts { get; set; } = null!;
    public DbSet<ProductCartItem> ProductCartItems { get; set; } = null!;
    public DbSet<ShopFollower> ShopFollowers { get; set; } = null!;
    public DbSet<Rating> Ratings { get; set; } = null!;
    public DbSet<Category> Categories { get; set; } = null!;
    public DbSet<Setting> Settings { get; set; } = null!;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
