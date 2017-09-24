using System.Threading.Tasks;

namespace Mazadaty.Services.Messaging
{
    public interface IMessageService
    {
        Task Send(Email email);
    }
}
