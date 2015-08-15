using Mzayad.Data;
using Mzayad.Models;
using Mzayad.Models.Enum;
using System.Data.Entity;
using System.Threading.Tasks;

namespace Mzayad.Services
{
    public class UserProfileService : ServiceBase
    {
        public UserProfileService(IDataContextFactory dataContextFactory) : base(dataContextFactory)
        {
        }

        public async Task<UserProfile> CreateNewProfile(ApplicationUser user)
        {
            using (var dc = DataContext())
            {
                var userProfile = new UserProfile
                {
                    UserId = user.Id,
                    Status = UserProfileStatus.Private,
                    ProfileUrl = UserProfile.GenerateProfileUrl(user.UserName),
                    Avatar = await dc.Avatars.FirstOrDefaultAsync()
                };
                
                dc.UserProfiles.Add(userProfile);
                await dc.SaveChangesAsync();
                return userProfile;
            }
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
                var userProfile = await dc.UserProfiles.Include(i => i.Avatar).SingleOrDefaultAsync(i => i.UserId == user.Id);
                if (userProfile == null)
                {
                    userProfile = await CreateNewProfile(user);
                }

                return userProfile;
            }
        }

        public async Task<UserProfile> GetByUserId(string userId)
        {
            using (var dc = DataContext())
            {
                var userProfile = await dc.UserProfiles.Include(i => i.Avatar).SingleOrDefaultAsync(i => i.UserId == userId);
                return userProfile;
            }
        }

        public async Task<UserProfile> Update(UserProfile userProfile)
        {
            using (var dc = DataContext())
            {
                dc.SetModified(userProfile);
                await dc.SaveChangesAsync();
                return userProfile;
            }
        }
    }
}
