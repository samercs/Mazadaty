using Mzayad.Core.Exceptions;
using Mzayad.Data;
using Mzayad.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Mzayad.Services
{
    public class AvatarService : ServiceBase
    {
        private readonly TokenService _tokenService;
        public AvatarService(IDataContextFactory dataContextFactory)
            : base(dataContextFactory)
        {
            _tokenService = new TokenService(dataContextFactory);
        }

        public async Task<IReadOnlyCollection<Avatar>> GetAll()
        {
            using (var dc = DataContext())
            {
                return await dc.Avatars.OrderBy(i => i.SortOrder).ToListAsync();
            }
        }

        public async Task<Avatar> GetById(int id)
        {
            using (var dc = DataContext())
            {
                return await dc.Avatars.SingleOrDefaultAsync(i => i.AvatarId == id);
            }
        }

        public async Task<Avatar> Add(Avatar avatar)
        {
            using (var dc = DataContext())
            {
                var sortOrder = 0.0;
                var allAvatars = await dc.Avatars.OrderBy(i => i.SortOrder).ToListAsync();
                if (allAvatars.Any())
                {
                    sortOrder = allAvatars.Last().SortOrder + 1;
                }
                avatar.SortOrder = sortOrder;

                dc.Avatars.Add(avatar);
                await dc.SaveChangesAsync();

                return avatar;
            }
        }

        public async Task<Avatar> Update(Avatar avatar)
        {
            using (var dc = DataContext())
            {
                dc.SetModified(avatar);
                await dc.SaveChangesAsync();
                return avatar;
            }
        }

        public async Task Delete(Avatar avatar)
        {
            using (var dc = DataContext())
            {
                dc.Avatars.Attach(avatar);
                dc.Avatars.Remove(avatar);
                await dc.SaveChangesAsync();
            }
        }

        public async Task<Avatar> Save(Avatar avatar)
        {
            using (var dc = DataContext())
            {
                dc.SetModified(avatar);
                await dc.SaveChangesAsync();
                return avatar;
            }
        }

        public async Task BuyAvatar(ApplicationUser user, Avatar avatar, string userHostAddress)
        {
            using (var dc = DataContext())
            {
                if (user.Tokens >= avatar?.Token)
                {
                    await _tokenService.RemoveTokensFromUser(user, avatar.Token, user, userHostAddress, "Buy Premium Avatar");
                }
                else
                {
                    throw new InsufficientTokensException("Avatar cannot be purchased, user does not have enough available tokens.");
                }
            }
        }

        public async Task ChangeAvatar(ApplicationUser user, Avatar avatar, string userHostAddress)
        {
            if (avatar.IsPremium)
            {
                try
                {
                    if (!await UserHasAvatar(user, avatar))
                    {
                        await BuyAvatar(user, avatar, userHostAddress);
                    }

                }
                catch (Exception e)
                {

                    throw;
                }
                await AddAvatarToUser(user, avatar);

            }
            else
            {

                await AddAvatarToUser(user, avatar);
            }
        }

        public async Task<bool> UserHasAvatar(ApplicationUser user, Avatar avatar)
        {
            using (var dc = DataContext())
            {
                var userAvatars =
                    await dc.UserAvatars.FirstOrDefaultAsync(i => i.AvatarId == avatar.AvatarId && i.UserId == user.Id);
                return userAvatars != null;
            }
        }

        public async Task AddAvatarToUser(ApplicationUser user, Avatar avatar)
        {
            using (var dc = DataContext())
            {
                var userAvatars =
                    await dc.UserAvatars.FirstOrDefaultAsync(i => i.AvatarId == avatar.AvatarId && i.UserId == user.Id);
                if (userAvatars == null)
                {
                    var userAvatar = new UserAvatar
                    {
                        UserId = user.Id,
                        AvatarId = avatar.AvatarId
                    };
                    dc.UserAvatars.Add(userAvatar);
                    await dc.SaveChangesAsync();
                }

            }
        }

        public async Task<IEnumerable<Avatar>> GetPremiumAvatar(ApplicationUser user = null)
        {
            using (var dc = DataContext())
            {
                var premiumAvatars = await dc.Avatars.Where(i => i.IsPremium).ToListAsync();
                if (user != null)
                {
                    var userAvatars = await GetUserAvatars(user);
                    var userAvatarIds = userAvatars.Select(i => i.AvatarId);
                    premiumAvatars = premiumAvatars.Where(i => !userAvatarIds.Contains(i.AvatarId)).ToList();
                }
                return premiumAvatars;
            }
        }

        public async Task<IEnumerable<Avatar>> GetUserAvatars(ApplicationUser user)
        {
            using (var dc = DataContext())
            {
                return await dc.UserAvatars
                    .Include(i => i.Avatar)
                    .Where(i => i.UserId == user.Id).Select(i => i.Avatar)
                    .ToListAsync();
            }
        }
    }
}
