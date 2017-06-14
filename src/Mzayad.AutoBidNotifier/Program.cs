using Microsoft.Azure.WebJobs;

namespace Mzayad.AutoBidNotifier
{
    internal class Program
    {
        private static void Main()
        {
            new JobHost().CallAsync(typeof(Functions).GetMethod("NotifyAutoBidUsers")).Wait();
        }
    }
}
