using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Mzayad.Models;
using Mzayad.Models.Enums;
using Mzayad.Services;
using Mzayad.Services.Identity;
using Mzayad.Web.Areas.Api.Models.Friends;
using Mzayad.Web.Core.Services;
using Mzayad.Web.Models.Friends;

namespace Mzayad.Web.Areas.Api.Controllers
{
    [RoutePrefix("api/friends")]
    [Authorize]
    public class FriendsController : ApplicationApiController
    {
        private readonly FriendService _friendService;
        private readonly UserService _userService;
        private readonly IAuthService _authService;
        private readonly MessageService _messageService;
        public FriendsController(IAppServices appServices) : base(appServices)
        {
            _friendService = new FriendService(DataContextFactory);
            _userService = new UserService(DataContextFactory);
            _authService = appServices.AuthService;
            _messageService = new MessageService(DataContextFactory);
        }

        [Route("requests")]
        [HttpGet]
        public async Task<IHttpActionResult> UserFriendsRequest()
        {
            var requests = await _friendService.GetFriendRequests(AuthService.CurrentUserId());
            var result = new
            {
                Requests = requests.Select(i => new
                {
                    i.FriendRequestId,
                    i.User.UserName,
                    i.FriendId,
                    i.UserId,
                    i.User.FirstName,
                    i.User.LastName
                })
            };
            return Ok(result);
        }

        [Route("sendrequest")]
        [HttpPost]
        public async Task<IHttpActionResult> SendFriendsRequest(FriendRequestModel model)
        {
            var user = await _userService.GetUserByName(model.UserName);
            if (user == null)
            {
                return NotFound();
            }
            await _friendService.SendRequest(new FriendRequest
            {
                UserId = _authService.CurrentUserId(),
                FriendId = user.Id,
                Status = FriendRequestStatus.NotDecided
            });
            return Ok(new { message = "friend request has been send successfully." });
        }

        [Route("updaterequest")]
        [HttpPost]
        public async Task<IHttpActionResult> UpdateFriendsRequest(UpdateFriendRequestModel model)
        {
            string message = string.Empty;
            var friendRequest = new FriendRequest
            {
                FriendRequestId = model.RequestId,
                Status = model.Status
            };
            if (model.Status == FriendRequestStatus.Accepted)
            {
                await _friendService.AcceptRequest(friendRequest);
                message = "Request has been accepted successfully.";
            }
            if (model.Status == FriendRequestStatus.Declined)
            {
                await _friendService.DeclineRequest(friendRequest);
                message = "Request has been decline successfully.";
            }
            return Ok(new { message });
        }

        [Route("notifications")]
        [HttpGet]
        public IHttpActionResult GetNotifications()
        {
            var friendRequestCount = _friendService.CountFriendRequests(AuthService.CurrentUserId());
            var messageCount = _messageService.CountNewMesssages(AuthService.CurrentUserId());
            return Ok(new { friendRequestCount, messageCount });
        }

        [Route("{userName}/message")]
        [HttpGet]
        public async Task<IHttpActionResult> FrindMessages(string userName)
        {
            var currentUser = await AuthService.CurrentUser();
            var user = await _userService.GetUserByName(userName);
            if (user == null)
            {
                return NotFound();
            }
            await _messageService.SetAllAsRead(currentUser.Id, user.Id);
            var history = await _messageService.GetHistory(currentUser.Id, user.Id);
            var result = new
            {
                User = new
                {
                    currentUser.UserName,
                    currentUser.FirstName,
                    currentUser.LastName,
                    currentUser.AvatarUrl
                },
                History = history.Select(i => new
                {
                    i.User.UserName,
                    i.User.FirstName,
                    i.User.LastName,
                    i.User.AvatarUrl,
                    i.IsNew,
                    i.MessageId,
                    i.Body,
                    i.Summary,
                    i.CreatedUtc
                })
            };
            return Ok(result);
        }

        [Route("{userName}/message")]
        [HttpPost]
        public async Task<IHttpActionResult> SendMessages(string userName, SendMessageViewModel model)
        {
            var user = await _userService.GetUserByName(userName);
            if (user == null)
            {
                return NotFound();
            }
            var message = new Message
            {
                UserId = AuthService.CurrentUserId(),
                ReceiverId = user.Id,
                IsNew = true,
                Body = model.Body
            };
            await _messageService.Insert(message);
            return Ok(new {message = "Message has been send successfully."});
        }
    }

    public class SendMessageViewModel
    {
        public string Body { get; set; }

    }
}