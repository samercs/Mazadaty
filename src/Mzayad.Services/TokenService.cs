using System.Data.Entity;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Mzayad.Data;
using Mzayad.Models;

namespace Mzayad.Services
{
    public class TokenService : ServiceBase
    {
        public TokenService(IDataContextFactory dataContextFactory) : base(dataContextFactory)
        {
        }

        public async Task UpdateUserTokens(ApplicationUser user, int tokens)
        {        
            var userStore = new UserStore<ApplicationUser>((DbContext)DataContext());
            var userManager = new UserManager<ApplicationUser>(userStore);

            //var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<DataContext>()));
        }
    }
}
