using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using Mzayad.Web.Core.Services;

namespace Mzayad.Web.Areas.Api.Controllers
{
    [Authorize]
    public class Test1Controller : ApiController
    {
        private IAuthService _authService { get; set; }
        
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
        private IAuthService _authService { get; set; }
        
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
