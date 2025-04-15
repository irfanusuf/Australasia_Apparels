using System;
using System.ComponentModel.DataAnnotations.Schema;
using P2WebMVC.Models.DomainModels;

namespace P2WebMVC.Models.JunctionModels;

public class OrderItems
{

    public Guid OrderItemId { get; set; } = Guid.NewGuid();
    public required Guid OrderId { get; set; }  // FK to Order
    [ForeignKey("OrderId")] // Foreign key to Order
    public Order? Order { get; set; }  // Navigation property to Order



    public required Guid ProductId { get; set; }  // FK to Product
    [ForeignKey("ProductId")] // Foreign key to Product
    public Product? Product { get; set; }  // Navigation property to Product
    public required int Quantity { get; set; } 
    
    

}
