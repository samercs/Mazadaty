using Mzayad.Models;
using System;
using System.Threading.Tasks;

namespace Mzayad.Services
{
    public class UserService
    {
        private readonly Func<ApplicationUser, Task> _updateUser;

        public UserService(Func<ApplicationUser, Task> updateUser)
        {
            _updateUser = updateUser;
        }

        public async Task DoSomething(ApplicationUser user)
        {
            user.FirstName += "!";

            await _updateUser(user);
        }
    }
}
