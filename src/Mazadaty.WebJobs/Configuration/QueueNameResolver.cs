using System;
using Microsoft.Azure.WebJobs;

namespace Mazadaty.WebJobs.Configuration
{
    public class QueueNameResolver : INameResolver
    {
        public string Resolve(string name)
        {
            var environment = Environment.GetEnvironmentVariable("WEBSITE_SLOT_NAME") ?? Environment.MachineName;
            return $"{name}-{environment}".ToLowerInvariant();
        }
    }
}
