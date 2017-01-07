using Mzayad.Models;
using Mzayad.Models.Enums;
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

        [HttpPost]
        [Route("cancelrequest")]
        public async Task<JsonResult> CancelRequest(int requestId)
        {
            await _friendService.CancelRequest(requestId);
            return Json("1", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("updaterequest")]
        public async Task<JsonResult> UpdateRequest(int requestId, FriendRequestStatus status)
        {
            var friendRequest = new FriendRequest()
            {
                FriendRequestId = requestId,
                Status = status
            };
            if (status == FriendRequestStatus.Accepted)
            {
                await _friendService.AcceptRequest(friendRequest);
            }
            if (status == FriendRequestStatus.Declined)
            {
                await _friendService.DeclineRequest(friendRequest);
            }
            return Json("1", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("remove")]
        public async Task<JsonResult> Remove(string friendId)
        {
            await _friendService.RemoveFriend(AuthService.CurrentUserId(), friendId);
            return Json("1", JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}