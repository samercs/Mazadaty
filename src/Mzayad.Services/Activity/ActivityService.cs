using Microsoft.WindowsAzure.Storage;

namespace Mzayad.Services.Activity
{
    public interface IActivityService
    {
        
    }

    public class ActivityService : IActivityService
    {
        public ActivityService(string connectionString)
        {
            var storageAccount = CloudStorageAccount.Parse(connectionString);
        }
    }
}
