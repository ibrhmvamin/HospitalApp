using Business.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Business.Concrete
{
    public class EmailService : IEmailService
    {
        public void SendEmail(string email, string subject, string body)
        {
            MailMessage mailMessage = new()
            {
                From = new MailAddress("mehemmed05.aliyev@gmail.com"),
                Subject = subject,
                Body=body,
                IsBodyHtml = true
            };
            mailMessage.To.Add(email);


            mailMessage.Body = body;

            SmtpClient smtpClient = new()
            {
                Port = 587,
                Host = "smtp.gmail.com",
                EnableSsl = true,
                Credentials = new NetworkCredential("mehemmed05.aliyev@gmail.com", "pmkt bkfz ntog qxrh")
            };

            smtpClient.Send(mailMessage);
        }

        public async Task SendReminderEmailAsync(string email, string subject, string body)
        {
            MailMessage mailMessage = new()
            {
                From = new MailAddress("mehemmed05.aliyev@gmail.com"),
                Subject = subject,
                Body=body,
                IsBodyHtml = true
            };

            mailMessage.To.Add(email);


            mailMessage.Body = body;

            SmtpClient smtpClient = new()
            {
                Port = 587,
                Host = "smtp.gmail.com",
                EnableSsl = true,
                Credentials = new NetworkCredential("mehemmed05.aliyev@gmail.com", "pmkt bkfz ntog qxrh")
            };

            smtpClient.Send(mailMessage);

            await Task.CompletedTask;
        }
    }
}
