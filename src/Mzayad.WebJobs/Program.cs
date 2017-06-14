using Microsoft.Azure.WebJobs;
using Mzayad.WebJobs.Configuration;

namespace Mzayad.WebJobs
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
