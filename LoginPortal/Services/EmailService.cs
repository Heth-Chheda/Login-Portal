using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using System.Net.Mail; // For MailAddress validation


//VCNBQRUM25TT46LGACVRKX57 --> mail server code

namespace LoginPortal.Services
{
    public class EmailService
    {
        private readonly string _smtpServer = "live.smtp.mailtrap.io"; // Mailtrap SMTP server
        private readonly string _smtpUsername = "smtp@mailtrap.io";
        private readonly string _smtpPassword = "7554f189b9fc304bc426faf4aee9214a";
        private readonly int _smtpPort = 2525;
        private readonly string _senderEmail = "hello@demomailtrap.com";  // Custom domain

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            string sanitizedEmail = toEmail?.Trim();

            try
            {
                var mailAddress = new System.Net.Mail.MailAddress(sanitizedEmail);
            }
            catch (FormatException)
            {
                Console.WriteLine($"Invalid email format: {sanitizedEmail}");
                throw new InvalidOperationException("The email address is not in a valid format.");
            }

            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("NoReply", _senderEmail));   // Custom domain sender
            emailMessage.To.Add(new MailboxAddress("", sanitizedEmail));
            emailMessage.Subject = subject;

            string emailBody = $@"
            <html>
                <body>
                    <h2>Welcome to our application!</h2>
                    <p>Thank you for registering with us. Please click the button below to verify your email address:</p>
                    <a href='{body}' style='background-color: #4CAF50; color: white; padding: 15px 32px; text-align: center; text-decoration: none; display: inline-block; font-size: 16px; border-radius: 8px;'>Verify Email</a>
                    <p>If you did not register, please ignore this email.</p>
                </body>
            </html>";

            emailMessage.Body = new TextPart("html")
            {
                Text = emailBody
            };

            try
            {
                using (var client = new MailKit.Net.Smtp.SmtpClient())
                {
                    await client.ConnectAsync(_smtpServer, _smtpPort, SecureSocketOptions.StartTls);
                    await client.AuthenticateAsync(_smtpUsername, _smtpPassword);
                    await client.SendAsync(emailMessage);
                    await client.DisconnectAsync(true);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send email: {ex.Message}");
                throw new InvalidOperationException("Failed to send email. Please try again later.");
            }
        }
    }

}
