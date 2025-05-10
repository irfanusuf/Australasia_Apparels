using System;

namespace P2WebMVC.Models.DomainModels;

public class NewsLetter
{

    public Guid NewsLetterId {get;set;} = Guid.NewGuid();

    public required  string Email {get;set;}


}
