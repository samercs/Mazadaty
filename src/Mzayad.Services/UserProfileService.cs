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

        public async Task<UserProfile> GetByUser(string id)
        {
            using (var dc = DataContext())
            {
                return await dc.UserProfiles.SingleOrDefaultAsync(i => i.UserId == id);
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
