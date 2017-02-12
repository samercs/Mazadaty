using Mzayad.Models;
using Mzayad.Models.Enums;
using Mzayad.Services;
using Mzayad.Services.Identity;
using Mzayad.Web.Core.Services;
using Mzayad.Web.Models.User;
using System.Net;
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
        private readonly MessageService _messageService;
        public FriendsController(IAppServices appServices)
            : base(appServices)
        {
            _friendService = new FriendService(appServices.DataContextFactory); ;
            _userService = new UserService(appServices.DataContextFactory);
            _authService = appServices.AuthService;
            _messageService = new MessageService(appServices.DataContextFactory);
        }

        public int RequestsCount()
        {
            return _friendService.CountFriendRequests(AuthService.CurrentUserId());
        }
        public int MessagesCount()
        {
            return _messageService.CountNewMesssages(AuthService.CurrentUserId());
        }

        [Route("requests")]
        public async Task<ActionResult> Requests()
        {
            var viewModel = new FriendRequestsViewModel()
            {
                OthersRequests = await _friendService.GetFriendRequests(AuthService.CurrentUserId()),
            };
            return View(viewModel);
        }

        [Route("friend/{userName}/message")]
        public async Task<ActionResult> SendMessage(string userName)
        {
            var user = await _userService.GetUserByName(userName);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(new Message() { User = user});
        }

        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
        [Route("friend/{userName}/message")]
        public async Task<ActionResult> SendMessage(string userName, Message model)
        {
            var reciever = await _userService.GetUserByName(userName);
            if (reciever == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            model.IsNew = true;
            model.UserId = AuthService.CurrentUserId();
            model.ReceiverId = reciever.Id;

            var message = await _messageService.Insert(model);
            TempData["MessageSent"] = true;
            return RedirectToAction("friends", "user");
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
                Status = FriendRequestStatus.NotDecided
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

        public async Task<MvcHtmlString> Message(int id)
        {
            var message = await _messageService.Get(id);
            await _messageService.SetAsRead(message);
            return MvcHtmlString.Create(message.Body);
        }
        #endregion
    }
}