namespace Backend.Services;

using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Identity.UI.Services;

public class EmailSender : IEmailSender
{
    private readonly IConfiguration _configuration;
    public EmailSender(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public async Task SendEmailAsync(string email, string subject, string message)
    {
        var mail = _configuration["Email"];
        var pwd = _configuration["EmailPassword"];
        var client = new SmtpClient( "smtp.gmail.com",587)
        {
            EnableSsl = true,
            Credentials = new NetworkCredential(mail, pwd)
        };
        await client.SendMailAsync(new MailMessage{ To = {email},IsBodyHtml = true, Subject = subject, Body = message, From = new MailAddress(mail)});       
    }
}