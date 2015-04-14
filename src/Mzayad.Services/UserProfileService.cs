using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mzayad.Data;
using Mzayad.Models;

namespace Mzayad.Services
{
    public class UserProfileService : ServiceBase
    {
        public UserProfileService(IDataContextFactory dataContextFactory) : base(dataContextFactory)
        {
        }

        public async Task<UserProfile> Add(UserProfile userProfile)
        {
            using (var dc=DataContext())
            {
                dc.UserProfiles.Add(userProfile);
                await dc.SaveChangesAsync();
                return userProfile;
            }
        }

        public async Task<UserProfile> GetByUser(ApplicationUser user)
        {
            using (var dc = DataContext())
            {
                var userProfile = await dc.UserProfiles.SingleOrDefaultAsync(i => i.UserId == user.Id);
                if (userProfile == null)
                {
                    userProfile = new UserProfile // create default user profile
                    {
                        UserId = user.Id,
                        Gamertag = user.UserName,
                        Avatar = await dc.Avatars.FirstOrDefaultAsync()
                    };
                }

                return userProfile;
            }
        }

        public async Task<bool> Exsist(string gamertag)
        {
            using (var dc=DataContext())
            {
                return (await dc.UserProfiles.Where(i => i.Gamertag == gamertag).ToArrayAsync()).Any();
            }
        }

        public async Task<UserProfile> Update(UserProfile userProfile)
        {
            using (var dc = DataContext())
            {
                dc.UserProfiles.Attach(userProfile);
                dc.SetModified(userProfile);
                await dc.SaveChangesAsync();
                return userProfile;
            }
        }
    }
}
