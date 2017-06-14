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

        public async Task<Message> SetAsRead(Message entity)
        {
            using (var dc = DataContext())
            {
                var message = dc.Messages.Single(i => i.MessageId == entity.MessageId);
                message.IsNew = false;
                await dc.SaveChangesAsync();
                return message;
            }
        }
        public async Task SetAllAsRead(string reciverId, string senderId)
        {
            using (var dc = DataContext())
            {
                var messages = dc.Messages
                    .Where(i => i.ReceiverId.Equals(reciverId))
                    .Where(i => i.UserId.Equals(senderId)).ToList();
                messages.ForEach(i =>
                {
                    i.IsNew = false;
                    dc.SetModified(i);
                });
                await dc.SaveChangesAsync();
            }
        }
        public int CountNewMesssages(string userId)
        {
            using (var dc = DataContext())
            {
                return dc.Messages
                    .Count(i => i.ReceiverId == userId && i.IsNew);
            }
        }
        public async Task<IReadOnlyCollection<Message>> GetBySender(string userId)
        {
            using (var dc = DataContext())
            {
                return await dc.Messages.Where(i => i.UserId == userId).ToListAsync();
            }
        }

        public async Task<IReadOnlyCollection<Message>> GetHistory(string senderId, string reciverId)
        {
            using (var dc = DataContext())
            {
                return await dc.Messages
                    .Include(i => i.User)
                    .Where(i => (i.UserId.Equals(senderId) && i.ReceiverId.Equals(reciverId)) ||
                    i.ReceiverId.Equals(senderId) && i.UserId.Equals(reciverId))
                    .OrderBy(i => i.CreatedUtc)
                    .ToListAsync();
            }
        }

        public async Task<IReadOnlyCollection<Message>> GetByReceiver(string userId)
        {
            using (var dc = DataContext())
            {
                return await dc.Messages
                    .Include(i => i.User)
                    .Where(i => i.ReceiverId == userId)
                    .OrderByDescending(i => i.CreatedUtc)
                    .ToListAsync();
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

        public async Task<Message> Get(int messageId)
        {
            using (var dc = DataContext())
            {
                return await dc.Messages.SingleAsync(i => i.MessageId == messageId);
            }
        }
    }
}
