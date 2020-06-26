using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.Runtime;
using System.IO;

namespace kpm_csharp_webform.Controllers
{
  public class HomeController : Controller
  {
    [HttpGet]
    public ActionResult Index()
    {
      return View();
    }

    [HttpPost]
    public ActionResult Index(string email)
    {
      UploadObjectUsingPresignedURLTest.Upload("file");
      //   SendEMail().Wait();
      ViewData["Message"] = $"An email message has been sent to <strong>{email}</strong> with a link to the file's new cloud location.";
      return View("Success");
    }

    static async Task SendEMail()
    {
      string apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
      SendGridClient client = new SendGridClient(apiKey);
      EmailAddress from = new EmailAddress("ken.p.mckinney@gmail.com", "Kent McKinney");
      string subject = "Sending with SendGrid is Fun";
      EmailAddress to = new EmailAddress("kent.p.mckinney@gmail.com", "Recipient");
      string plainTextContent = "and easy to do anywhere, even with C#";
      string htmlContent = "<strong>and easy to do anywhere, even with C#</strong>";
      SendGridMessage msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
      SendGrid.Response response = await client.SendEmailAsync(msg);
    }
  }
}
