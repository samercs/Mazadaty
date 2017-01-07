using Mzayad.Data;
using Mzayad.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Mzayad.Services
{
    public class FriendService : ServiceBase
    {
        public FriendService(IDataContextFactory dataContextFactory)
            : base(dataContextFactory)
        { }

        public async Task<FriendRequest> SendRequest(FriendRequest entity)
        {
            using (var dc = DataContext())
            {
                if (!dc.FriendsRequests.Any(i => i.UserId == entity.UserId && i.FriendId == entity.FriendId))
                {
                    dc.FriendsRequests.Add(entity);
                    await dc.SaveChangesAsync();
                }
                return entity;
            }
        }

        public async Task<FriendRequest> AcceptRequest(FriendRequest entity)
        {
            using (var dc = DataContext())
            {
                var request = dc.FriendsRequests.SingleOrDefault(i => i.FriendRequestId == entity.FriendRequestId);
                request.Status = Models.Enums.FriendRequestStatus.Accepted;

                //add friend record(requester as user and requested as friend)
                dc.UsersFriends.Add(new UserFriend()
                {
                    UserId = request.UserId,
                    FriendId = request.FriendId,
                });

                //add friend record(requester as friend and requested as user)
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

        public async Task RemoveFriend(string userId, string friendId)
        {
            using (var dc = DataContext())
            {
                var friends = dc.UsersFriends.Where(i => (i.UserId == userId && i.FriendId == friendId)
                                                || (i.FriendId == userId && i.UserId == friendId));
                friends.ToList().ForEach(i => dc.UsersFriends.Remove(i));
                await dc.SaveChangesAsync();
            }
        }

        public async Task<IReadOnlyCollection<ApplicationUser>> GetFriends(string userId)
        {
            using (var dc = DataContext())
            {
                return await dc.UsersFriends
                                    .Include(i => i.User)
                                    .Where(i => i.UserId == userId)
                                    .Select(i => i.Friend)
                                    .OrderBy(i => i.UserName)
                                    .ToListAsync();
            }
        }
        public async Task<IReadOnlyCollection<FriendRequest>> GetFriendRequests(string userId)
        {
            using (var dc = DataContext())
            {
                return await dc.FriendsRequests
                                    .Where(i => i.FriendId == userId && i.Status == Models.Enums.FriendRequestStatus.NotDecided)
                                    .Include(i => i.Friend)
                                    .OrderBy(i => i.CreatedUtc)
                                    .ToListAsync();
            }
        }
        public async Task<IReadOnlyCollection<FriendRequest>> GetUserRequests(string userId)
        {
            using (var dc = DataContext())
            {
                return await dc.FriendsRequests
                                    .Where(i => i.UserId == userId && i.Status == Models.Enums.FriendRequestStatus.NotDecided)
                                    .Include(i => i.Friend)
                                    .OrderBy(i => i.CreatedUtc)
                                    .ToListAsync();
            }
        }
    }
}
