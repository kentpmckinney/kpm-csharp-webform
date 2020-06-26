using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Net.Http.Headers;
using System.Threading.Tasks;

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

        // Save the file uploaded from the client locally on the server
        using (var stream = new FileStream(path, FileMode.Create))
        {
          await file.CopyToAsync(stream);
        }

        // Upload the file to AWS S3
        string url = AWS.Upload(path);

        // Prepare messages to present to the user
        string subject = $"URL for Uploaded File '{fileName}'";
        string htmlBody = $@"
          <div>Thank you for using the Cloud File Uploader!</div>
          <ul>
            <li>Current Time: {DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss")}</li>
            <li>File Uploaded: {fileName}</li>
          </ul>
          <br/>
          The uploaded file may be accessed <a href='{url}'>here</a> for the next 30 minutes.
        ";
        string plainBody = $@"
          Thank you for using the Cloud File Uploader!\n\n
          Current Time: {DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss")}\n
          File Uploaded: {fileName}\n\n
          The uploaded file may be accessed for the next 30 minutes here: \n\n{url}
        ";

        // Send an email to the provided address
        Email.Send(email, subject, plainBody, htmlBody).Wait();

        // Present the Success view
        ViewData["Message"] = htmlBody + $@"
          <br/>An email message with this information has been sent to <strong>{email}</strong>.
        ";
        return View("Success");
      }
      else
      {
        // Present the Failure view
        ViewData["Message"] = $@"
          Please ensure the file selected is readable and try again.
        ";
        return View("Failure");
      }
    }
  }
}
