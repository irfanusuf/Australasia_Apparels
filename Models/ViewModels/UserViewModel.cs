using System;

namespace P2WebMVC.Models.ViewModels;

public class UserViewModel
{
 public List<User> Users { get; set; }= [];

 public User ? User { get; set; } 
}
