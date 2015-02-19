using System;
using System.Linq;
using System.Threading.Tasks;
using Mzayad.Web.Controllers;
using Mzayad.Web.Core.Services;
using System.Web.Mvc;
using Mzayad.Web.Areas.admin.Models.Users;
using Mzayad.Web.Core.Identity;

namespace Mzayad.Web.Areas.admin.Controllers
{
    public class UsersController : ApplicationController
    {
        public UsersController(IControllerServices controllerServices) : base(controllerServices)
        {
        }

        public async Task<ActionResult> Index(string search = "", string role = "")
        {
            var roles = from Role r in Enum.GetValues(typeof(Role))
                        orderby r.ToString()
                        select new { Id = r, Name = r.ToString() };

            var viewModel = new IndexViewModel
            {
                Search = search,
                Users = await AuthService.GetUsers(search, role),
                Role = role,
                Roles = new SelectList(roles, "Id", "Name")
            };

            return View(viewModel);
        }
	}
}