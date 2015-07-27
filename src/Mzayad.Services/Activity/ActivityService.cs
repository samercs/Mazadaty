using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Mzayad.Models;
using Mzayad.Models.Enums;
using Newtonsoft.Json;

namespace Mzayad.Services.Activity
{
    public interface IActivityService
    {
        Task AddActivity(ActivityType activityType, string userId);
    }

    public class ActivityService : IActivityService
    {
        private const string QueueName = "activities";
        private readonly CloudStorageAccount _storageAccount;

        public ActivityService(string connectionString)
        {
            _storageAccount = CloudStorageAccount.Parse(connectionString);  
        }

        public async Task AddActivity(ActivityType activityType, string userId)
        {
            var client = _storageAccount.CreateCloudQueueClient();
            var queue = client.GetQueueReference(QueueName);
            queue.CreateIfNotExists();

            var message = new CloudQueueMessage(JsonConvert.SerializeObject(new ActivityEvent
            {
                Type = activityType,
                UserId = userId
            }));

            await queue.AddMessageAsync(message);
        }
    }
}
