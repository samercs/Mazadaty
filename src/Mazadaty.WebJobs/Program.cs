using Microsoft.Azure.WebJobs;
using Mazadaty.WebJobs.Configuration;

namespace Mazadaty.WebJobs
{
    internal class Program
    {
        private static void Main()
        {
            var host = new JobHost(new JobHostConfiguration
            {
                NameResolver = new QueueNameResolver()
            });
            host.RunAndBlock();
        }
    }
}
