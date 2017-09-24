using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Mazadaty.Data;
using System.Data.Entity;

namespace Mazadaty.Services.Identity
{
    public class RoleManager : RoleManager<IdentityRole>
    {
        public RoleManager(IRoleStore<IdentityRole, string> store) : base(store)
        {
        }

        public RoleManager(IDataContextFactory dataContextFactory)
            : base(new RoleStore<IdentityRole>((DbContext) dataContextFactory.GetContext()))
        {       
        }
    }
}
