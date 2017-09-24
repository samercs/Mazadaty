using Mazadaty.Services;
using Mazadaty.Web.Core.Services;
using System.Threading.Tasks;
using System.Web.Http;

namespace Mazadaty.Web.Areas.Api.Controllers
{
    [RoutePrefix("api/avatars")]
    public class AvatarsController : ApplicationApiController
    {
        private readonly AvatarService _avatarService;

        public AvatarsController(IAppServices appServices) : base(appServices)
        {
            _avatarService = new AvatarService(appServices.DataContextFactory);
        }

        [Route("")]
        public async Task<IHttpActionResult> Get()
        {
            var freeAvatars = await _avatarService.GetFreeAvatars();
            return Ok(freeAvatars);
        }
    }
}
