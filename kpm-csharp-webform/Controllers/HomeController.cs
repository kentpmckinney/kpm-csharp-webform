using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;
using MailKit.Net.Smtp;
using MimeKit;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace kpm_csharp_webform.Controllers
{
  public class HomeController : Controller
  {
    public string Index()
    {
      // using (var client = new SmtpClient())
      // {
      // client.Connect(_appSettings.SmtpServerAddress, _appSettings.SmtpServerPort, SecureSocketOptions.StartTlsWhenAvailable);
      // client.AuthenticationMechanisms.Remove("XOAUTH2"); // Must be removed for Gmail SMTP
      // client.Authenticate(_appSettings.SmtpServerUser, _appSettings.SmtpServerPass);
      // client.Send(Email);
      // client.Disconnect(true);
      // }
      //   var message = new MimeMessage();
      //   var bodyBuilder = new BodyBuilder();
      //   message.From.Add(new MailboxAddress("Kent McKinney", "ken.p.mckinney@gmail.com"));
      //   message.To.Add(new MailboxAddress("Recipient", "ken.p.mckinney@gmail.com"));
      //   message.ReplyTo.Add(new MailboxAddress("Kent McKinney", "kent.p.mckinney@gmail.com"));
      //   message.Subject = "Your file has been uploaded and is ready for download";
      //   bodyBuilder.HtmlBody = "html body";
      //   message.Body = bodyBuilder.ToMessageBody();
      //   var client = new SmtpClient();
      //   client.ServerCertificateValidationCallback = (s, c, h, e) => true;
      //   client.Connect("smtp.gmail.com", 465, SecureSocketOptions.SslOnConnect);
      //   string pwd = System.Environment.GetEnvironmentVariable("EMAIL_PWD");
      //   try
      //   {
      //     client.Authenticate("ken.p.mckinney@gmail.com", pwd);
      //   }
      //   catch
      //   {
      //     ;
      //   }

      //   client.Send(message);
      //   client.Disconnect(true);
      //   return "test: " + pwd;//(IActionResult)
      //                         // return View();
      string key = System.Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
      return "this is a test";
    }
  }
}
