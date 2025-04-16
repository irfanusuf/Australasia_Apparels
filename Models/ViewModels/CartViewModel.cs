using System;
using P2WebMVC.Models.DomainModels;
using P2WebMVC.Models.JunctionModels;

namespace P2WebMVC.Models.ViewModels;

public class CartViewModel
{

public List<CartItem> CartItems { get; set; } = []  ;
public Cart? Cart { get; set; }

}
