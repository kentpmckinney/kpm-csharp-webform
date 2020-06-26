using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using SendGrid;
using SendGrid.Helpers.Mail;

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
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index([FromForm(Name = "email")] string email, [FromForm(Name = "file")] IFormFile file)
    {
      if (file != null)
      {
        var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');

        var path = Path.Combine(
            Directory.GetCurrentDirectory(), "wwwroot",
            fileName);
        Console.WriteLine(path);

        using (var stream = new FileStream(path, FileMode.Create))
        {
          await file.CopyToAsync(stream);
        }

        string url = UploadObjectUsingPresignedURLTest.Upload(path);

        //   SendEMail().Wait();
        ViewData["Message"] = $@"
            The uploaded file may be accessed <a href='{url}'>here</a> for the next 30 minutes.
            An email message has been sent to <strong>{email}</strong>
            with a link to the file's new cloud location.
          ";
        return View("Success");
      }
      else
      {
        ViewData["Message"] = $"Please select Browse and choose a file to upload.";
        return View("Failure");
      }






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
