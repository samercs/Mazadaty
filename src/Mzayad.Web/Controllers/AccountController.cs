using System.Web.Mvc;
using Mzayad.Web.Core.Services;

namespace Mzayad.Web.Controllers
{
    [Authorize]
    public class AccountController : ApplicationController
    {
        public AccountController(IControllerServices controllerServices) : base(controllerServices)
        {
        }
    }
}