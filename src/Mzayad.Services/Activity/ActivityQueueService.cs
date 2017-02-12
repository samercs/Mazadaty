using System;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Mzayad.Models;
using Mzayad.Models.Enums;
using Newtonsoft.Json;

namespace Mzayad.Services.Activity
{
    public interface IActivityQueueService
    {
        void QueueActivity(ActivityType activityType, string userId, string language = "en");
        Task QueueActivityAsync(ActivityType activityType, string userId, string language = "en");
        Task QueueActivityAsync(ActivityType activityType, string userId, int xp, string language = "en");
    }

    public class ActivityQueueService : IActivityQueueService
    {
        private readonly string _queueName = "activities";
        private readonly CloudStorageAccount _storageAccount;

        public ActivityQueueService(string connectionString)
        {
            _storageAccount = CloudStorageAccount.Parse(connectionString);

            if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("WEBSITE_SITE_NAME")))
            {
                _queueName = $"{_queueName}-{Environment.MachineName}".ToLowerInvariant();
            }
        }

        public void QueueActivity(ActivityType activityType, string userId, string language = "en")
        {
            var message = new CloudQueueMessage(JsonConvert.SerializeObject(new ActivityEvent
            {
                Type = activityType,
                UserId = userId,
                Language = language
            }));

            CreateQueue().AddMessageAsync(message);
        }

        public async Task QueueActivityAsync(ActivityType activityType, string userId, string language = "en")
        {
            var message = new CloudQueueMessage(JsonConvert.SerializeObject(new ActivityEvent
            {
                Type = activityType,
                UserId = userId,
                Language = language
            }));

            await CreateQueue().AddMessageAsync(message);
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

            await CreateQueue().AddMessageAsync(message);
        }

        private CloudQueue CreateQueue()
        {
            var client = _storageAccount.CreateCloudQueueClient();
            var queue = client.GetQueueReference(_queueName);
            queue.CreateIfNotExists();
            return queue;
        }
    }
}
