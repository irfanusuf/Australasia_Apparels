using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.NetworkInformation;
using P2WebMVC.Types;

namespace P2WebMVC.Models.DomainModels;

public class Order
{


[Key]

public required Guid OrderId { get; set; } = Guid.NewGuid();

public required Guid UserId { get; set; }  // Fk 

public User? User { get; set; }

public required Guid ProductId { get; set; }  // Fk
public Product? Product { get; set; }
public required int Quantity { get; set; } = 1;
public required decimal TotalPrice { get; set; } = 0;
public required DateTime DateCreated { get; set; } = DateTime.UtcNow;
public required OrderStatus OrderStatus { get; set; } = OrderStatus.Pending;
public required DateTime? ShippedDate { get; set; } =DateTime.UtcNow.AddDays(7);


}
