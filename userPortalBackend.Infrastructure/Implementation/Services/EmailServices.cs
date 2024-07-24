using Microsoft.Extensions.Configuration;
using MailKit.Net.Smtp;
using MimeKit;
using userPortalBackend.Application.DTO;
using userPortalBackend.Application.IServices;
using userPortalBackend.presentation.TempModels;
using userPortalBackend.Application.IRepository;


namespace userPortalBackend.Infrastructure.Implementation.Services
{
    public class EmailServices : IEmailServices
    {
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;
        public EmailServices(IConfiguration configuration, IUserRepository userRepository) { 
          _configuration = configuration;
          _userRepository = userRepository;
        }

        public void sendEmail(EmailDTO emailDTO)
        {
            var emailMsg = new MimeMessage();
            var from = _configuration["Emailsettings:From"];

            emailMsg.From.Add(new MailboxAddress("forgot-password", from));
            emailMsg.To.Add(new MailboxAddress(emailDTO.to, emailDTO.to));
            emailMsg.Subject = emailDTO.subject;
            Console.WriteLine($"Email Body: {emailDTO.body}");

            emailMsg.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = emailDTO.body
            };

            using (var client = new SmtpClient())
            {
                try
                {
                    //Parse the port number from configuration

                   int port = int.Parse(_configuration["Emailsettings:Port"]);

                    client.Connect(_configuration["Emailsettings:SmtpServer"], port, true);
                    client.Authenticate(_configuration["Emailsettings:Username"], _configuration["Emailsettings:Password"]);

                    client.Send(emailMsg);
                }
                catch (Exception ex)
                {
                   // Log or handle the exception as needed
                    Console.WriteLine($"Error sending email: {ex.Message}");
                    throw;
                }
                finally
                {
                     client.Disconnect(true);
                }
            }
        }
      


        public async Task<ResetPassword> setEmailToken(ResetPassword emailCredential)
        {
            return await _userRepository.setEmailToken(emailCredential);
        }
    }
}
