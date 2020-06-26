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
      if (file != null && file.Length > 0)
      {
        var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
        var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", fileName);

        using (var stream = new FileStream(path, FileMode.Create))
        {
          await file.CopyToAsync(stream);
        }

        string url = UploadObjectUsingPresignedURLTest.Upload(path);
        string subject = $"URL for Uploaded File '{fileName}'";
        string htmlBody = $@"
          <ul>
            <li>Current Time: {DateTime.Now.ToLongDateString()}</li>
            <li>File Uploaded: {fileName}</li>
          </ul>
          <br/>
          The uploaded file may be accessed <a href='{url}'>here</a> for the next 30 minutes.
          <br/>
          An email message with this information has been sent to <strong>{email}</strong>.
        ";
        string plainBody = $@"
          Current Time: {DateTime.Now.ToLongDateString()}\n
          File Uploaded: {fileName}\n\n
          The uploaded file may be accessed for the next 30 minutes here: \n\n{url}\n\n
          An email message with this information has been sent to '{email}'.
        ";

        SendEMail(email, subject, plainBody, htmlBody).Wait();
        ViewData["Message"] = htmlBody;
        return View("Success");
      }
      else
      {
        ViewData["Message"] = $@"
          Please select Browse and choose a file to upload and try again.
        ";
        return View("Failure");
      }
    }

    static async Task SendEMail(string toAddress, string subject, string plainBody, string htmlBody)
    {
      string apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
      SendGridClient client = new SendGridClient(apiKey);
      EmailAddress from = new EmailAddress("ken.p.mckinney@gmail.com", "Kent McKinney");
      EmailAddress to = new EmailAddress(toAddress, "File Upload User");
      SendGridMessage msg = MailHelper.CreateSingleEmail(from, to, subject, plainBody, htmlBody);
      SendGrid.Response response = await client.SendEmailAsync(msg);
    }
  }
}
