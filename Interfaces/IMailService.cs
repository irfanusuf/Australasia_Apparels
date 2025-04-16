using System;

namespace P2WebMVC.Interfaces;

public interface IMailService
{
Task SendEmailAsync(string to, string subject, string body, bool isHtml = true);
}
