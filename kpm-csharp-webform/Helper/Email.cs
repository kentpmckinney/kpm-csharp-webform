using System;
using System.Threading.Tasks;
using SendGrid;
using SendGrid.Helpers.Mail;

class Email
{
  static public async Task Send(string toAddress, string subject, string plainBody, string htmlBody)
  {
    string apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
    SendGridClient client = new SendGridClient(apiKey);
    EmailAddress from = new EmailAddress("ken.p.mckinney@gmail.com", "Kent McKinney");
    EmailAddress to = new EmailAddress(toAddress, "File Upload User");
    SendGridMessage msg = MailHelper.CreateSingleEmail(from, to, subject, plainBody, htmlBody);
    SendGrid.Response response = await client.SendEmailAsync(msg);
  }
}
