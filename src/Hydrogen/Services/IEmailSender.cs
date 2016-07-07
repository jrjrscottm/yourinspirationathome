using System.Threading;
using System.Threading.Tasks;
using Hydrogen.Core.Domain.Multitenancy;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace Hydrogen.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }

    public class SendGridEmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;
        private readonly ApplicationTenant _tenant;

        public SendGridEmailSender(IConfiguration configuration, ApplicationTenant tenant)
        {
            _configuration = configuration;
            _tenant = tenant;
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var myMessage = new MimeMessage();
            myMessage.From.Add(new MailboxAddress("No Reply", _tenant.Email.FromAddress));
            myMessage.To.Add(new MailboxAddress(email, email));
            myMessage.Subject = subject;

            myMessage.Body = new TextPart("plain")
            {
                Text = message
            };

            using (var smtpClient = new SmtpClient())
            {
                smtpClient.Connect("smtp.sendgrid.net", 587, SecureSocketOptions.Auto, CancellationToken.None);
                smtpClient.AuthenticationMechanisms.Remove("XOAUTH2");
                smtpClient.Authenticate(_configuration["email:username"], _configuration["email:password"]);

                await smtpClient.SendAsync(myMessage);
                smtpClient.Disconnect(true);
            }
        }
    }
}
