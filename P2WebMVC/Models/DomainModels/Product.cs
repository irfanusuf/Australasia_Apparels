using System;
using System.ComponentModel.DataAnnotations;
using P2WebMVC.Types;

namespace P2WebMVC.Models.DomainModels;

public class Product
{

[Key]
public required Guid ProductId { get; set; } = Guid.NewGuid();
public required string ProductName { get; set; }
public required string ProductDescription { get; set; }
public required string ProductImage { get; set; }
public required decimal ProductPrice { get; set; }
public required int ProductStock { get; set; }
public required ProductCategory Category { get; set; } = ProductCategory.General;
public required bool IsDeleted { get; set; } = false;
public required bool IsActive { get; set; } = true;
public required DateTime CreatedAt { get; set; } = DateTime.UtcNow;
public required DateTime UpdatedAt { get; set; } = DateTime.UtcNow;



}
