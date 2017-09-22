using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;

namespace Mzayad.Services.Messaging
{
    public class EmailMessageService: IMessageService
    {
        private readonly EmailSettings _emailSettings;
        public EmailMessageService(EmailSettings emailSettings)
        {
            _emailSettings = emailSettings;
        }
        public async Task Send(Email email)
        {
            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = email.Message;
            bodyBuilder.TextBody = email.Message;
            var message = new MimeMessage()
            {
                Subject = email.Subject,
                Body = bodyBuilder.ToMessageBody()
            };
            message.To.Add(new MailboxAddress(email.ToAddress, email.ToAddress));
            message.From.Add(new MailboxAddress(_emailSettings.FromName, _emailSettings.FromEmail));
            using (var client = new SmtpClient())
            {
                client.Connect("smtp.gmail.com", 587, false);
                client.Authenticate(_emailSettings.FromEmail, _emailSettings.EmailPassword);
                await client.SendAsync(message);
                client.Disconnect(true);
            }
        }
    }
}