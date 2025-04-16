using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using P2WebMVC.Models.DomainModels;
using P2WebMVC.Types;

namespace P2WebMVC.Models;

public class User
{

    [Key]
    public  Guid UserId {get;set;} = Guid.NewGuid();
    public required string  Username {get ;set;}
    public required string  Email {get ;set;}
    public required string  Password {get ;set;}
    public string?  ProfilePicUrl {get ;set;}
    public string? Phone {get; set ; }
    public Role Role {get;set;} = Role.User;
    public Address? Address { get; set; }
    public Cart? Cart { get; set; }
    public ICollection<Order> Orders { get; set; } = [];


    
   
    public DateTime DateCreated { get; set; }  = DateTime.UtcNow;
    public DateTime? DateModified { get; set; } = DateTime.UtcNow;
 


}
