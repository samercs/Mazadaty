using Mzayad.Data;
using Mzayad.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Mzayad.Services
{
    public class MessageService : ServiceBase
    {
        public MessageService(IDataContextFactory dataContextFactory)
            : base(dataContextFactory)
        { }

        public async Task<Message> Insert(Message entity)
        {
            using (var dc = DataContext())
            {
                dc.Messages.Add(entity);
                await dc.SaveChangesAsync();
                return entity;
            }
        }

        public async Task<Message> Update(Message entity)
        {
            using (var dc = DataContext())
            {
                var message = dc.Messages.Single(i => i.MessageId == entity.MessageId);
                message.IsNew = false;
                await dc.SaveChangesAsync();
                return message;
            }
        }

        public async Task<IReadOnlyCollection<Message>> GetBySender(string userId)
        {
            using (var dc = DataContext())
            {
                return await dc.Messages.Where(i => i.UserId == userId).ToListAsync();
            }
        }
        public async Task<IReadOnlyCollection<Message>> GetByReceiver(string userId)
        {
            using (var dc = DataContext())
            {
                return await dc.Messages.Where(i => i.ReceiverId == userId).ToListAsync();
            }
        }
        public async Task Delete(Message entity)
        {
            using (var dc = DataContext())
            {
                dc.Messages.Attach(entity);
                dc.Messages.Remove(entity);
                await dc.SaveChangesAsync();
            }
        }
    }
}
