using Mzayad.Web.Core.Services;
using Mzayad.Web.Extensions;
using OrangeJetpack.Services.Client.Messaging;
using OrangeJetpack.Services.Models;
using System.Threading.Tasks;
using System.Web.Http;

namespace Mzayad.Web.Areas.Api.Controllers
{
    public class TestController : ApiController
    {
        private readonly IMessageService _messageService;
        
        public TestController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        [Route("~/api/test")]
        public async Task<IHttpActionResult> Get()
        {
            var email = new Email
            {
                ToAddress = "andy.mehalick@orangejetpack.com",
                Subject = "test",
                Message = "test"
            };

            var result = await _messageService.Send(email.WithTemplate());

            return Ok(result);
        }
    }
     
    [Authorize]
    public class Test1Controller : ApiController
    {
        private readonly IAuthService _authService;
        
        public Test1Controller(IAuthService authService)
        {
            _authService = authService;
        }
        
        public async Task<IHttpActionResult> Get()
        {
            var user = await _authService.GetUserByName(User.Identity.Name);
            
            return Ok(user);
        }
    }

    public class Test2Controller : ApiController
    {
        private readonly IAuthService _authService;
        
        public Test2Controller(IAuthService authService)
        {
            _authService = authService;
        }
        
        public IHttpActionResult Get()
        {
            return Ok(_authService.IsAuthenticated().ToString());
        }
    }
}
