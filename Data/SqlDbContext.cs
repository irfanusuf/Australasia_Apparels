using System;
using Microsoft.EntityFrameworkCore;
using P2WebMVC.Models;
using P2WebMVC.Models.DomainModels;
using P2WebMVC.Models.JunctionModels;

namespace P2WebMVC.Data;

public class SqlDbContext : DbContext
{

    public SqlDbContext(DbContextOptions<SqlDbContext> options) : base(options) { }

    //entities

    public DbSet<User> Users { get; set; }
    public DbSet<Address> Addresses { get; set; }
    public DbSet<Cart> Carts { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<CartItem> CartItems { get; set; }
    public DbSet<OrderItems> OrderItems { get; set; }




    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

            modelBuilder.Entity<Order>()
                .HasOne(u => u.Username)
                .IsUnique();


        // Configure the composite key for CartItem
        modelBuilder.Entity<CartItem>()
            .HasKey(ci => new { ci.CartItemId, ci.CartId, ci.ProductId });

        // Configure the composite key for OrderItems
        modelBuilder.Entity<OrderItems>()
            .HasKey(oi => new { oi.OrderItemId, oi.OrderId, oi.ProductId });

        // Configure the one-to-many relationship between User and Orders
        modelBuilder.Entity<User>()
            .HasMany(u => u.Orders)
            .WithOne(o => o.Buyer)
            .HasForeignKey(o => o.UserId);



    }
}