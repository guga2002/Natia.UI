using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Configuration;
using Natia.Persistance.Interface;

namespace Natia.Persistance.Repositories
{
    public class SmtpClientRepository : ISmtpClientRepository
    {
        private readonly IConfiguration _config;

        public SmtpClientRepository(IConfiguration Config)
        {
            _config = Config;
        }

        public async Task SendMessage(string body, string to, string Subject)
        {
            try
            {
                //var smtp = _config.GetSection("EmailSettings") ?? throw new ArgumentException("No smtp config present");
                using var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("globaltvmanagment@gmail.com", "reoiuyqgeipepngo"),
                    EnableSsl = true
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress("globaltvmanagment@gmail.com"),
                    Subject = $"{Subject} {DateTime.Now}",
                    Body = body,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(to);

                await smtpClient.SendMailAsync(mailMessage);
                Console.WriteLine("Message sent successfully to Guga.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send email: {ex.Message}");
            }
        }
        public async Task SendMessage(string body)
        {
            try
            {
                //var smtp = _config.GetSection("SmtpSettings") ?? throw new ArgumentException("No smtp config present");
                using var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("globaltvmanagment@gmail.com", "reoiuyqgeipepngo"),
                    EnableSsl = true
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress("globaltvmanagment@gmail.com" ?? ""),
                    Subject = $"გუგა პრობლემა  გუგა: {DateTime.Now}",
                    Body = body,
                    IsBodyHtml = true
                };

                mailMessage.To.Add("aapkhazava22@gmail.com");

                await smtpClient.SendMailAsync(mailMessage);
                Console.WriteLine("Message sent successfully to Guga.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send email: {ex.Message}");
            }
        }
        public string BuildHtmlMessage(string message, string stackTrace)
        {
            return $@"
<html>
    <head>
        <style>
            body {{
                font-family: Arial, sans-serif;
                line-height: 1.6;
                background-color: #f9f9f9;
                color: #333;
                padding: 20px;
            }}
            .container {{
                max-width: 600px;
                margin: 0 auto;
                background: #fff;
                border: 1px solid #ddd;
                border-radius: 8px;
                padding: 20px;
                box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
            }}
            h2 {{
                color: #e74c3c;
                border-bottom: 2px solid #e74c3c;
                padding-bottom: 5px;
            }}
            .problem {{
                margin: 20px 0;
                padding: 10px;
                background-color: #fbe9e7;
                border-left: 4px solid #e74c3c;
                color: #e74c3c;
            }}
            .stacktrace {{
                background-color: #f4f4f4;
                border: 1px solid #ddd;
                padding: 10px;
                border-radius: 5px;
                white-space: pre-wrap;
                font-family: Consolas, monospace;
            }}
            .footer {{
                margin-top: 20px;
                font-size: 0.9em;
                color: #666;
            }}
        </style>
    </head>
    <body>
        <div class='container'>
            <h2>🚨 პრობლემა გვაქვს</h2>
            <p>გუგა,</p>
            <p>ვეღარ ვსაუბრობ, ნათია ვარ:</p>
            <div class='problem'>
                <strong>ეს შეცდომა გვაქვს:</strong> {message}
            </div>
            <p><strong>დეტალურად:</strong></p>
            <div class='stacktrace'>{stackTrace}</div>
            <p><strong>დამატებით ინფორმაცია:</strong></p>
            <div class='stacktrace'>გუგა მეც ვცდილობ გამოსწორებას, სერვისების სიცოცხლე გადავამოწმე, როცა მოიცლი გადაამოწმე</div>
            <p class='footer'>
                ეს  ესემესი არის გადაუდებელი სიტუაციებისთვის გათვლილი, გთხოვ გადაამოწმო<br>
                <em>ნათია ჯანდაგიშვილი</em>
            </p>
        </div>
    </body>
</html>";
        }
    }
}
