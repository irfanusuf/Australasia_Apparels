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
        public DbSet<OrderItem> OrderItems { get; set; }




        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {



                modelBuilder.Entity<Address>()
                        .HasOne(a => a.Buyer)
                        .WithOne(u => u.Address)
                        .HasForeignKey<Address>(a => a.UserId);



                modelBuilder.Entity<Cart>()
                        .HasOne(c => c.Buyer)
                        .WithOne(u => u.Cart)
                        .HasForeignKey<Cart>(c => c.UserId);


                modelBuilder.Entity<Order>()
                        .HasOne(o => o.Buyer)
                        .WithMany(u => u.Orders)
                        .HasForeignKey(o => o.UserId).OnDelete(DeleteBehavior.NoAction);



               modelBuilder.Entity<Order>()
                         .HasOne(o => o.Address)
                         .WithMany(a => a.Orders) // ✅ No navigation property on Address, or use WithMany(a => a.Orders)
                         .HasForeignKey(o => o.AddressId)
                        .OnDelete(DeleteBehavior.NoAction);




                modelBuilder.Entity<CartItem>()
                        .HasKey(ci => new { ci.CartId, ci.ProductId });



                modelBuilder.Entity<CartItem>()
                        .HasOne(ci => ci.Cart)
                        .WithMany(c => c.CartItems)
                        .HasForeignKey(ci => ci.CartId);




                modelBuilder.Entity<CartItem>()
                        .HasOne(ci => ci.Product)
                        .WithMany(p => p.CartItems)
                        .HasForeignKey(ci => ci.ProductId);




                modelBuilder.Entity<OrderItem>()
                        .HasKey(oi => new { oi.OrderId, oi.ProductId });


                modelBuilder.Entity<OrderItem>()
                        .HasOne(oi => oi.Order)
                        .WithMany(o => o.OrderItems)
                        .HasForeignKey(oi => oi.OrderId);



                modelBuilder.Entity<OrderItem>()
                        .HasOne(oi => oi.Product)
                        .WithMany(p => p.OrderItems)
                        .HasForeignKey(oi => oi.ProductId);

        }
}