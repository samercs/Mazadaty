using Mazadaty.Models.Enums;

namespace Mazadaty.Web.Areas.Api.Models.Friends
{
    public class UpdateFriendRequestModel
    {
        public int RequestId { get; set; }
        public FriendRequestStatus Status { get; set; }
    }
}
