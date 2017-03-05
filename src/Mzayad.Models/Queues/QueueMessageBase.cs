using Newtonsoft.Json;

namespace Mzayad.Models.Queues
{
    public abstract class QueueMessageBase
    {
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}