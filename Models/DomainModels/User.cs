using System;
using System.ComponentModel.DataAnnotations;
using P2WebMVC.Models.DomainModels;

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

    public Address? Address { get; set; } 

    public Cart? Cart { get; set; }
    public ICollection<Order> Orders { get; set; } = [];
   
    public DateTime DateCreated { get; set; }  = DateTime.UtcNow;
    public DateTime? DateModified { get; set; } = DateTime.UtcNow;
 


}
