using Microsoft.AspNet.Identity;
using Mzayad.Web.Core.Services;
using Mzayad.Web.Models.User;
using System.Threading.Tasks;
using System.Web.Http;

namespace Mzayad.Web.Areas.Api.Controllers
{
    [RoutePrefix("api/user")]
    public class UserController : ApiController
    {
        private readonly IAuthService _authService;

        public UserController(IAuthService authService)
        {
            _authService = authService;
        }

        public IHttpActionResult Get()
        {
            return Ok("sure");
        }

        [HttpGet, Authorize, Route("test")]
        public IHttpActionResult Test()
        {
            return Ok("yup");
        }

        [Route("change-password")]
        public async Task<IHttpActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.ChangePassword(User.Identity, model.CurrentPassword, model.NewPassword);
            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        private IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (result.Succeeded)
            {
                return null;
            }
            
            if (result.Errors != null)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error);
                }
            }

            if (ModelState.IsValid)
            {
                // No ModelState errors are available to send, so just return an empty BadRequest.
                return BadRequest();
            }

            return BadRequest(ModelState);
        }
    }
}
