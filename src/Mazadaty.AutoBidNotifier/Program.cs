using Microsoft.Azure.WebJobs;

namespace Mazadaty.AutoBidNotifier
{
    internal class Program
    {
        private static void Main()
        {
            new JobHost().CallAsync(typeof(Functions).GetMethod("NotifyAutoBidUsers")).Wait();
        }
    }
}
