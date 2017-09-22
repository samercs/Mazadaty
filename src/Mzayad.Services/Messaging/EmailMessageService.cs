using System.Threading.Tasks;

namespace Mzayad.Services.Messaging
{
    public class EmailMessageService: IMessageService
    {
        private readonly EmailSettings _emailSettings;
        public EmailMessageService(EmailSettings emailSettings)
        {
            _emailSettings = emailSettings;
        }
        public Task Send(Email email)
        {
            throw new System.NotImplementedException();
        }
    }
}