using Mzayad.Web.Controllers;
using OrangeJetpack.Base.Web;
using OrangeJetpack.Services.Models;

namespace Mzayad.Web.Extensions
{
    public static class ControllerExtensions
    {
        public static Email WithTemplate(this Email email, ApplicationController controller)
        {
            var messageWithTemplate = ControllerUtilities.GetPartialViewAsString(controller, "EmailTemplate", email);
            email.Message = messageWithTemplate;

            return email;
        }
    }
}