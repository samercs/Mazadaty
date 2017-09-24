using Newtonsoft.Json;

namespace Mazadaty.Models.Queues
{
    public abstract class QueueMessageBase
    {
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
