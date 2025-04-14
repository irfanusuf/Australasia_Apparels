using System;
using System.ComponentModel.DataAnnotations;

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
    public string? Address  {get; set ; }


}
