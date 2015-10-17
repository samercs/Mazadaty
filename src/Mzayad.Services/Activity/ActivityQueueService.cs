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
        Task QueueActivity(ActivityType activityType, string userId,int xp);
    }

    public class ActivityQueueService : IActivityQueueService
    {
        private const string QueueName = "activities";
        private readonly CloudStorageAccount _storageAccount;

        public ActivityQueueService(string connectionString)
        {
            _storageAccount = CloudStorageAccount.Parse(connectionString);  
        }

        public async Task QueueActivity(ActivityType activityType, string userId , int xp)
        {
            var client = _storageAccount.CreateCloudQueueClient();
            var queue = client.GetQueueReference(QueueName);
            queue.CreateIfNotExists();

            var message = new CloudQueueMessage(JsonConvert.SerializeObject(new ActivityEvent
            {
                Type = activityType,
                UserId = userId,
                XP = xp
            }));

            await queue.AddMessageAsync(message);
        }
    }
}
