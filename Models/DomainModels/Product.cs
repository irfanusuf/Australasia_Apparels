using System;
using System.ComponentModel.DataAnnotations;
using P2WebMVC.Models.JunctionModels;
using P2WebMVC.Types;

namespace P2WebMVC.Models.DomainModels;

public class Product
{ 

[Key]
public required Guid ProductId { get; set; } = Guid.NewGuid();
public required string Name { get; set; }
public required string Description { get; set; }
public required string ImageUrl { get; set; }
public required decimal Price { get; set; }
public required decimal Discount { get; set; }
public required int Stock { get; set; }
public required ProductCategory Category { get; set; } = ProductCategory.All;
public required string SubCategory { get; set; }
public required string Brand { get; set; }
public required string Color { get; set; }
public required string Size { get; set; }
public required bool IsDeleted { get; set; } = false;
public required bool IsActive { get; set; } = true;
public ICollection<CartItem> CartItems { get; set; } = [];
public ICollection<OrderItem> OrderItems { get; set; } = [];
public required DateTime CreatedAt { get; set; } = DateTime.UtcNow;
public required DateTime UpdatedAt { get; set; } = DateTime.UtcNow;



}
