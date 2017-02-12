using System;
using Microsoft.Azure.WebJobs;

namespace Mzayad.WebJobs
{
    public class QueueNameResolver : INameResolver
    {
        public string Resolve(string name)
        {
            if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("WEBSITE_SITE_NAME")))
            {
                name = $"{name}-{Environment.MachineName}".ToLowerInvariant();
            }

            return name;
        }
    }
}