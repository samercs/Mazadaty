using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Mzayad.Models;
using Mzayad.Models.Enums;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace Mzayad.Services.Activity
{
    public interface IQueueService
    {
        void LogBid(Bid bid);

        void QueueActivity(ActivityType activityType, string userId, string language = "en");
        Task QueueActivityAsync(ActivityType activityType, string userId, string language = "en");
        Task QueueActivityAsync(ActivityType activityType, string userId, int xp, string language = "en");
    }

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

            CreateQueue("bids").AddMessage(message);
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
