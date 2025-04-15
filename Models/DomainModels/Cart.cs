using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using P2WebMVC.Models.JunctionModels;

namespace P2WebMVC.Models.DomainModels;

public class Cart
{

    [Key]
    public Guid CartId { get; set; } = Guid.NewGuid();
    public required Guid UserId { get; set; } // FK

    [ForeignKey("UserId")]
    public User? Buyer { get; set; }
    public int CartValue {get;set;}

    public ICollection<CartItem> CartItems { get; set; } = [];
    public DateTime DateCreated { get; set; } = DateTime.UtcNow;
    public DateTime? DateModified { get; set; } = DateTime.UtcNow;


}
