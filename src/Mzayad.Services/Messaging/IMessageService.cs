using System.Threading.Tasks;

namespace Mzayad.Services.Messaging
{
    public interface IMessageService
    {
        Task Send(Email email);
    }
}