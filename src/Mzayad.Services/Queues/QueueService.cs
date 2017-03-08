using System;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Mzayad.Models;
using Mzayad.Models.Enum;
using Mzayad.Models.Enums;
using Mzayad.Models.Queues;
using Newtonsoft.Json;

namespace Mzayad.Services.Queues
{
    public class QueueService : IQueueService
    {
        private readonly CloudQueueClient _cloudQueueClient;
        private readonly string _environmentName;

        public QueueService(string connectionString)
        {
            var storageAccount = CloudStorageAccount.Parse(connectionString);
            _cloudQueueClient = storageAccount.CreateCloudQueueClient();
            _environmentName = Environment.GetEnvironmentVariable("WEBSITE_SLOT_NAME") ?? Environment.MachineName;
        }

        public void LogBid(Bid bid)
        {
            var message = new CloudQueueMessage(JsonConvert.SerializeObject(new 
            {
                bid.BidId,
                bid.AuctionId,
                bid.UserId,
                bid.Type
            }));

            CreateQueue(QueueKeys.Bids).AddMessage(message);
        }

        public async Task LogTrophy(ApplicationUser user, TrophyKey trophyKey)
        {
            var userTrophy = new UserTrophyMessage(user.Id, trophyKey);
            var message = new CloudQueueMessage(userTrophy.ToJson());

            await CreateQueue(QueueKeys.Trophies).AddMessageAsync(message);
        }

        public void QueueActivity(ActivityType activityType, string userId, string language = "en")
        {
            var message = new CloudQueueMessage(JsonConvert.SerializeObject(new ActivityEvent
            {
                Type = activityType,
                UserId = userId,
                Language = language
            }));

            CreateQueue("activities").AddMessage(message);
        }

        public async Task QueueActivityAsync(ActivityType activityType, string userId, string language = "en")
        {
            var message = new CloudQueueMessage(JsonConvert.SerializeObject(new ActivityEvent
            {
                Type = activityType,
                UserId = userId,
                Language = language
            }));

            await CreateQueue("activities").AddMessageAsync(message);
        }

        public async Task QueueActivityAsync(ActivityType activityType, string userId, int xp, string language = "en")
        {
            var message = new CloudQueueMessage(JsonConvert.SerializeObject(new ActivityEvent
            {
                Type = activityType,
                UserId = userId,
                XP = xp,
                Language = language
            }));

            await CreateQueue("activities").AddMessageAsync(message);
        }

        private CloudQueue CreateQueue(string name)
        {
            var queueName = $"{name}-{_environmentName}".ToLowerInvariant();
            var cloudQueue = _cloudQueueClient.GetQueueReference(queueName);
            cloudQueue.CreateIfNotExists();
            return cloudQueue;
        }
    }
}
