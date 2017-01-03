using Mzayad.Data;
using Mzayad.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Mzayad.Services
{
    public class FriendService:ServiceBase
    {
        public FriendService(IDataContextFactory dataContextFactory)
            : base(dataContextFactory)
        { }

        public async Task<FriendRequest> SendRequest(FriendRequest entity)
        {
            using (var dc = DataContext())
            {
                dc.FriendsRequests.Add(entity);
                await dc.SaveChangesAsync();
                return entity;
            }
        }

        public async Task<FriendRequest> AcceptRequest(FriendRequest entity)
        {
            using (var dc = DataContext())
            {
                var request = dc.FriendsRequests.SingleOrDefault(i => i.FriendRequestId == entity.FriendRequestId);
                request.Status = Models.Enums.FriendRequestStatus.Accepted;

                //add friends records(requester as user and requested as friend)
                dc.UsersFriends.Add(new UserFriend()
                {
                    UserId = request.UserId,
                    FriendId = request.FriendId,
                });
                
                //add friends records(requester as friend and requested as user)
                dc.UsersFriends.Add(new UserFriend()
                {
                    FriendId = request.UserId,
                    UserId = request.FriendId,
                });

                await dc.SaveChangesAsync();
                return entity;
            }
        }
        public async Task<FriendRequest> DeclineRequest(FriendRequest entity)
        {
            using (var dc = DataContext())
            {
                var request = dc.FriendsRequests.SingleOrDefault(i => i.FriendRequestId == entity.FriendRequestId);
                request.Status = Models.Enums.FriendRequestStatus.Declined;
                await dc.SaveChangesAsync();
                return entity;
            }
        }
    }
}
