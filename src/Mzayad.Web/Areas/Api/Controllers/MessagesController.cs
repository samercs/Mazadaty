using System.Threading.Tasks;
using System.Web.Http;
using Mzayad.Services.Messaging;
using Mzayad.Web.Extensions;


namespace Mzayad.Web.Areas.Api.Controllers
{
    public class MessagesController : ApiController
    {
        private readonly IMessageService _messageService;

        public MessagesController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        [HttpPost]
        public async Task<IHttpActionResult> Post([FromBody] Email email)
        {
            await _messageService.Send(email.WithTemplate());
            return Ok();
        }
    }
}
