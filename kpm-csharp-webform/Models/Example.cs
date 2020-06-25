
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Threading.Tasks;

namespace kpm_csharp_webform
{
  internal class Example
  {
    public async Task Execute(string From, string To, string Subject, string Body)
    {
      string apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
      apiKey = "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";
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
