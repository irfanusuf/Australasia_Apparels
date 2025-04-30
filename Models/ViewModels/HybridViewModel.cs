using System;
using P2WebMVC.Models.DomainModels;
using P2WebMVC.Models.JunctionModels;

namespace P2WebMVC.Models.ViewModels;

public class HybridViewModel
{

    public Cart? Cart {get; set;}
    public  List<CartItem> CartItems {get; set;} =[];


   
    public  List<Order> Orders {get; set;} =[];
    public  List<OrderItem> OrderItems {get; set;} =[];    


    public Order? Order {get; set;}

    public Address? Address {get; set;}


}
