using System;
using P2WebMVC.Models.DomainModels;

namespace P2WebMVC.Models.ViewModels;

public class OrderViewModel
{


 public List<Order> Orders { get; set; }= [];

 public Order ? Order { get; set; } 


}
