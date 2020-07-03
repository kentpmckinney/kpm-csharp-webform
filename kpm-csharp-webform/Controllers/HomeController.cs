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
    public async Task<IActionResult> Index([FromForm(Name = "email")] string email, [FromForm(Name = "file")] IFormFile file, [FromForm(Name = "timezone")] string timezone)
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
        if (!System.IO.File.Exists(path))
          throw new FileNotFoundException($"File '{file.Name}' failed to upload");

        // Check that the file is not too large
        FileInfo fileInfo = new FileInfo(path);
        if (fileInfo.Length > 10 * 1024 * 1024)
          throw new FileLoadException("File being uploaded must be less than or equal to 10 MiB");

        // Upload the file to AWS S3
        string url = AWS.Upload(path);
        if (url == null || url == "")
          throw new ArgumentNullException("The file download URL must not be null or empty");

        // Prepare messages to present to the user
        string subject = $"URL for Uploaded File '{fileName}'";
        DateTime utcTime = DateTime.UtcNow;
        TimeZoneInfo localTimeZone = TimeZoneInfo.FindSystemTimeZoneById(timezone);
        DateTime pacificTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, localTimeZone);
        string htmlBody = $@"
          <div>Thank you for using Kent's Cloud Share!</div>
          <ul>
            <li>Current Time: {pacificTime.ToString("MM/dd/yyyy HH:mm:ss")} ({timezone})</li>
            <li>File Uploaded: {fileName}</li>
          </ul>
          <br/>
          The uploaded file may be accessed for the next 60 minutes.
          <br />
          To download the file, click <a href='{url}'>here</a>.
        ";
        string plainBody = $@"
          Thank you for using the Cloud File Uploader!\n\n
          Current Time: {pacificTime.ToString("MM/dd/yyyy HH:mm:ss")} ({timezone})\n
          File Uploaded: {fileName}\n\n
          The uploaded file may be accessed for the next 60 minutes.\n
          To download the file, click here: \n\n{url}
        ";

        // Send an email to the provided address
        Task<string> t = Email.Send(email, subject, plainBody, htmlBody);
        t.Wait();
        string r = t.Result;
        if (r == "Accepted")
        {
          // Present the Success view
          ViewData["Message"] = htmlBody + $@"
            <br/>An email message with this information has been sent to <strong>{email}</strong>.
          ";
          return View("Success");
        }
        else
        {
          // Present the Success view
          ViewData["Message"] = htmlBody + $@"
            <br/><span class='alert alert-warning'>There was an error sending a message to <strong>{email}</strong></span>.
          ";
          return View("Success");
        }
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
