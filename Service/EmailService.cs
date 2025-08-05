using Microsoft.Extensions.Options;
using System.Net.Mail;
using System.Net;
using TravelDesk.Interface;
using TravelDesk.Models;

namespace TravelDesk.Service
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;

        public EmailService(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            using (var client = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.SmtpPort))
            {
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(_emailSettings.SenderEmail, _emailSettings.SenderPassword);
                client.EnableSsl = true; // Ensure this matches the port and server requirements

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_emailSettings.SenderEmail),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };
                mailMessage.To.Add(toEmail);

                try
                {
                    await client.SendMailAsync(mailMessage);
                }
                catch (Exception ex)
                {
                    // Log the exception details
                    Console.WriteLine($"Failed to send email: {ex.Message}");
                    throw; // Rethrow the exception to be caught by the controller
                }
            }
        }

        public async Task SendEmailWithAttachmentAsync(string toEmail, string subject, string body, byte[] attachmentBytes, string attachmentFileName)
        {
            using (var client = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.SmtpPort))
            {
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(_emailSettings.SenderEmail, _emailSettings.SenderPassword);
                client.EnableSsl = true;

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_emailSettings.SenderEmail),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };
                mailMessage.To.Add(toEmail);

                // Add PDF attachment
                if (attachmentBytes != null && attachmentBytes.Length > 0)
                {
                    using (var attachmentStream = new MemoryStream(attachmentBytes))
                    {
                        var attachment = new Attachment(attachmentStream, attachmentFileName, "application/pdf");
                        mailMessage.Attachments.Add(attachment);
                    }
                }

                try
                {
                    await client.SendMailAsync(mailMessage);
                }
                catch (Exception ex)
                {
                    // Log the exception details
                    Console.WriteLine($"Failed to send email with attachment: {ex.Message}");
                    throw; // Rethrow the exception to be caught by the controller
                }
            }
        }
    }
}

