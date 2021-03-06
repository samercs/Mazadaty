using System.Collections.Generic;
using System.Linq;
using Mazadaty.Models;
using Mazadaty.Models.Enums;
using Mazadaty.Services;
using Mazadaty.Services.Identity;
using Mazadaty.Web.Core.Services;
using Mazadaty.Web.Models.Friends;
using Mazadaty.Web.Models.User;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Mazadaty.Web.Controllers
{
    [RoutePrefix("{language}")]
    public class FriendsController : ApplicationController
    {
        private readonly FriendService _friendService;
        private readonly UserService _userService;
        private readonly IAuthService _authService;
        private readonly MessageService _messageService;
        private readonly TrophyService _trophyService;
        public FriendsController(IAppServices appServices)
            : base(appServices)
        {
            _friendService = new FriendService(appServices.DataContextFactory); ;
            _userService = new UserService(appServices.DataContextFactory);
            _authService = appServices.AuthService;
            _messageService = new MessageService(appServices.DataContextFactory);
            _trophyService = new TrophyService(DataContextFactory);
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
            var currentUser = await AuthService.CurrentUser();
            var user = await _userService.GetUserByName(userName);
            if (user == null)
            {
                return HttpNotFound();
            }
            await _messageService.SetAllAsRead(currentUser.Id, user.Id);
            var history = await _messageService.GetHistory(currentUser.Id, user.Id);
            var model = new SendMessageViewModel
            {
                Message = new Message() { User = user },
                History = history,
                User = currentUser
            };
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
        [Route("friend/{userName}/message")]
        public async Task<ActionResult> SendMessage(string userName, SendMessageViewModel model)
        {
            var reciever = await _userService.GetUserByName(userName);
            if (reciever == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            model.Message.IsNew = true;
            model.Message.UserId = AuthService.CurrentUserId();
            model.Message.ReceiverId = reciever.Id;

            var message = await _messageService.Insert(model.Message);
            TempData["MessageSent"] = true;
            return RedirectToAction("SendMessage", "Friends", new { userName });
        }

        [Route("friends/search")]
        [HttpPost, ValidateAntiForgeryToken]
        [Authorize]
        public async Task<ActionResult> Search(string username)
        {
            var user = await AuthService.CurrentUser();
            var frinds = await _friendService.SearchByUserName(username, user);
            var model = new SearchFriendViewModel
            {
                SearchResult = await GetFriendsModel(frinds),
                Query = username
            };
            return View(model);
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

        private async Task<IEnumerable<UserProfileViewModel>> GetFriendsModel(IEnumerable<ApplicationUser> users)
        {
            var result = new List<UserProfileViewModel>();
            foreach (var user in users)
            {
                result.Add(new UserProfileViewModel(user)
                {
                    Trophies = await _trophyService.GetTrophies(user.Id, Language),
                    AreFriends = await _friendService.AreFriends(user.Id, AuthService.CurrentUserId()),
                    SentFriendRequestBefore = await _friendService.SentBefore(AuthService.CurrentUserId(), user.Id),
                    Friends = await _friendService.GetFriends(user.Id),
                    Me = user.UserName == (await AuthService.CurrentUser()).UserName
                });
            }
            return result;

        }
    }

    public class SearchFriendViewModel
    {
        public IEnumerable<UserProfileViewModel> SearchResult { get; set; }
        public string Query { get; set; }

    }
}
