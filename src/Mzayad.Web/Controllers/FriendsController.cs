using Mzayad.Models;
using Mzayad.Services;
using Mzayad.Services.Identity;
using Mzayad.Web.Core.Services;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Mzayad.Web.Controllers
{
    [RoutePrefix("{language}")]
    public class FriendsController : ApplicationController
    {
        private readonly FriendService _friendService;
        private readonly UserService _userService;
        private readonly IAuthService _authService;
        public FriendsController(IAppServices appServices) 
            : base(appServices)
        {
            _friendService = new FriendService(appServices.DataContextFactory); ;
            _userService = new UserService(appServices.DataContextFactory);
            _authService = appServices.AuthService;
        }

        #region Ajax Calls
        [HttpPost]
        [Route("sendrequest")]
        public async Task<JsonResult> SendRequest(string userName)
        {
            var user = await _userService.GetUserByName(userName);
            if (user == null)
            {
                return Json("-1", JsonRequestBehavior.AllowGet);
            }
            await _friendService.SendRequest(new FriendRequest()
            {
                UserId = _authService.CurrentUserId(),
                FriendId = user.Id,
                Status = Mzayad.Models.Enums.FriendRequestStatus.NotDecided
            });
            return Json("1", JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}