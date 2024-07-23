using Microsoft.Extensions.Configuration;
using MailKit.Net.Smtp;
using MimeKit;
using userPortalBackend.Application.DTO;
using userPortalBackend.Application.IServices;


namespace userPortalBackend.Infrastructure.Implementation.Services
{
    public class EmailServices : IEmailServices
    {
        private readonly IConfiguration _configuration;
        public EmailServices(IConfiguration configuration) { 
          _configuration = configuration;
        }

        public void sendEmail(EmailDTO emailDTO)
        {
            var emailMsg = new MimeMessage();
            var from = _configuration["Emailsettings:From"];
            emailMsg.From.Add(new MailboxAddress("forgot-pasword", from));
            emailMsg.To.Add(new MailboxAddress(emailDTO.to, emailDTO.to));
            emailMsg.Subject= emailDTO.subject;
            emailMsg.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = string.Format(emailDTO.body)
            };

            using (var client = new SmtpClient())
            {
                try
                {
                    client.Connect(_configuration["Emailsettings:Smtpserver"], 465, true);
                    client.Authenticate(_configuration["Emailsettings:Username"], _configuration["Emailsettings:Password"]);

                    client.Send(emailMsg);
                   
                }
                catch (Exception ex) {
                    throw;
                }
                finally
                {
                    client.Disconnect(true);
                }
            }

        }
    }
}
