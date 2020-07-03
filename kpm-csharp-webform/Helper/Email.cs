using System;
using System.Threading.Tasks;
using SendGrid;
using SendGrid.Helpers.Mail;

class Email
{
  static public async Task<string> Send(string toAddress, string subject, string plainBody, string htmlBody)
  {
    if (String.IsNullOrWhiteSpace(toAddress))
      return "";
    if (String.IsNullOrWhiteSpace(subject))
      return "";
    if (String.IsNullOrWhiteSpace(plainBody))
      return "";
    if (String.IsNullOrWhiteSpace(htmlBody))
      return "";
    if (Environment.GetEnvironmentVariable("SENDGRID_API_KEY") == null)
      throw new ArgumentNullException("The environment variable SENDGRID_API_KEY must not be null");
    string apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
    if (apiKey == "")
      throw new ArgumentException("The environment variable SENDGRID_API_KEY must not be empty");

    SendGridClient client = new SendGridClient(apiKey);
    EmailAddress from = new EmailAddress("ken.p.mckinney@gmail.com", "Kent McKinney");
    EmailAddress to = new EmailAddress(toAddress, "File Upload User");
    SendGridMessage msg = MailHelper.CreateSingleEmail(from, to, subject, plainBody, htmlBody);
    SendGrid.Response response = await client.SendEmailAsync(msg);
    string status = response.StatusCode.ToString();
    return status;
  }
}
