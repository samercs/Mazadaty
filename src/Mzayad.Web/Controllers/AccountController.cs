using System.Web.Mvc;
using Mzayad.Web.Core.Configuration;
using Mzayad.Web.Core.Services;
using Mzayad.Web.Models.Account;

namespace Mzayad.Web.Controllers
{
    [Authorize, RoutePrefix("{languageCode}/account")]
    public class AccountController : ApplicationController
    {
        public AccountController(IControllerServices controllerServices) : base(controllerServices)
        {
        }

        [AllowAnonymous, Route("sign-in")]
        public ActionResult SignIn(string returnUrl, int? shipmentId)
        {
            var viewModel = new SignInViewModel
            {
                ReturnUrl = returnUrl,
                Email = CookieService.Get(CookieKeys.LastSignInEmail)
            };

            return View(viewModel);
        }
    }
}