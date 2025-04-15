
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using P2WebMVC.Models.JunctionModels;
using P2WebMVC.Types;

namespace P2WebMVC.Models.DomainModels;

public class Order
{

[Key]
public required Guid OrderId { get; set; } = Guid.NewGuid();
public required OrderStatus OrderStatus { get; set; } = OrderStatus.Pending;
public required decimal TotalPrice { get; set; } = 0;
public required Guid UserId { get; set; }  // Fk 

[ForeignKey("UserId")]
public User? Buyer { get; set; }

public ICollection<OrderItem> OrderItems { get; set; } = [];

public required DateTime DateCreated { get; set; } = DateTime.UtcNow;
public required DateTime? ShippingDate { get; set; } =DateTime.UtcNow.AddDays(7);


}
