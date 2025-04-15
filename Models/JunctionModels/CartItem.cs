using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using P2WebMVC.Models.DomainModels;

namespace P2WebMVC.Models.JunctionModels;

public class CartItem
{
[Key]
    public Guid CartItemId { get; set; } = Guid.NewGuid();
    public Guid CartId { get; set; } // FK
    [ForeignKey("CartId")]  
    public Cart? Cart { get; set; } // Navigation property



    public Guid ProductId { get; set; } // FK

    [ForeignKey("ProductId")]
    public Product? Product { get; set; } // Navigation property


    public int Quantity { get; set; } // Quantity of the product in the cart
 
}
