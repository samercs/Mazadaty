using Mazadaty.Data;
using Mazadaty.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Mazadaty.Models.Enums;
using Mazadaty.Services.Identity;

namespace Mazadaty.Services
{
    public class FriendService : ServiceBase
    {


        public FriendService(IDataContextFactory dataContextFactory)
            : base(dataContextFactory)
        {
        }

        public async Task<FriendRequest> SendRequest(FriendRequest entity)
        {
            using (var dc = DataContext())
            {
                var currentRequests = await dc.FriendsRequests
                    .Where(i => i.UserId == entity.UserId)
                    .Where(i => i.FriendId == entity.FriendId)
                    .Where(i => i.Status == FriendRequestStatus.NotDecided)
                    .ToArrayAsync();

                if (!currentRequests.Any())
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
                if (request == null)
                {
                    return null;
                }

                request.Status = FriendRequestStatus.Accepted;
                dc.SetModified(request);
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

        public async Task DeclineRequest(FriendRequest entity)
        {
            using (var dc = DataContext())
            {
                var request = dc.FriendsRequests.SingleOrDefault(i => i.FriendRequestId == entity.FriendRequestId);
                if (request == null)
                {
                    return;
                }

                request.Status = Models.Enums.FriendRequestStatus.Declined;
                dc.SetModified(request);
                await dc.SaveChangesAsync();
            }
        }

        public async Task RemoveFriend(string userId, string friendId)
        {
            using (var dc = DataContext())
            {
                //delete friend request record
                var friendRequests = dc.FriendsRequests
                    .Where(i => (i.UserId == userId && i.FriendId == friendId)
                                                || (i.FriendId == userId && i.UserId == friendId));
                friendRequests.ToList().ForEach(i => dc.FriendsRequests.Remove(i));

                //delete friends records
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
                                    .Include(i => i.User)
                                    .OrderBy(i => i.CreatedUtc)
                                    .ToListAsync();
            }
        }
        public int CountFriendRequests(string userId)
        {
            using (var dc = DataContext())
            {
                return dc.FriendsRequests
                                    .Count(i => i.FriendId == userId && i.Status == Models.Enums.FriendRequestStatus.NotDecided);
            }
        }
        //public async Task<IReadOnlyCollection<FriendRequest>> GetUserRequests(string userId)
        //{
        //    using (var dc = DataContext())
        //    {
        //        return await dc.FriendsRequests
        //                            .Where(i => i.UserId == userId && i.Status == Models.Enums.FriendRequestStatus.NotDecided)
        //                            .Include(i => i.Friend)
        //                            .OrderBy(i => i.CreatedUtc)
        //                            .ToListAsync();
        //    }
        //}

        public async Task CancelRequest(int requestId)
        {
            using (var dc = DataContext())
            {
                var friendRequest = dc.FriendsRequests.SingleOrDefault(i => i.FriendRequestId == requestId);
                dc.FriendsRequests.Remove(friendRequest);
                await dc.SaveChangesAsync();
            }
        }

        public async Task<bool> AreFriends(string userId, string friendId)
        {
            using (var dc = DataContext())
            {
                return await dc.UsersFriends.AnyAsync(i => (i.UserId == userId && i.FriendId == friendId)
                                                           ||
                                                           (i.UserId == friendId && i.FriendId == userId));
            }
        }

        public async Task<bool> SentBefore(string userId, string friendId)
        {
            using (var dc = DataContext())
            {
                return await dc.FriendsRequests.AnyAsync(i => i.UserId == userId
                                                            && i.FriendId == friendId
                                                            && i.Status == Models.Enums.FriendRequestStatus.NotDecided);
            }
        }

        public async Task<IEnumerable<ApplicationUser>> SearchByUserName(string username, ApplicationUser user)
        {
            using (var dc = DataContext())
            {
                var userManager = new UserManager(DataContextFactory);
                var users = await userManager.Users.Where(i => i.UserName.Contains(username) &&
                                                               i.Id != user.Id).ToListAsync();
                return users;
            }

        }
    }

}
