using System;
using System.ComponentModel.DataAnnotations.Schema;
using P2WebMVC.Models.DomainModels;
using P2WebMVC.Types;

namespace P2WebMVC.Models.JunctionModels;

public class OrderItem
{


    public  Guid OrderId { get; set; }  // FK to Order
    [ForeignKey("OrderId")] // Foreign key to Order
    public Order? Order { get; set; }  // Navigation property to Order



    public  Guid ProductId { get; set; }  // FK to Product
    [ForeignKey("ProductId")] // Foreign key to Product
    public Product? Product { get; set; }  // Navigation property to Product



    public required int Quantity { get; set; } 
    public ProductSize? Size {get;set;}
    public string? Color  {get;set;}

}
