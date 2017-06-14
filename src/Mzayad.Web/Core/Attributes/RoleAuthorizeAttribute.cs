using System;
using System.Linq;
using System.Web.Mvc;
using Mzayad.Web.Controllers;
using Mzayad.Web.Core.Identity;
using OrangeJetpack.Base.Web;

namespace Mzayad.Web.Core.Attributes
{
    internal class RoleAuthorizeAttribute : AuthorizeAttribute
    {
        public RoleAuthorizeAttribute(params Role[] roles)
        {
            Roles = string.Join(",", roles.Select(r => Enum.GetName(r.GetType(), r)));
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            var controller = filterContext.Controller as ApplicationController;
            if (controller != null)
            {
                controller.SetStatusMessage("We're sorry but you are not authorized to view this page.", StatusMessageType.Error);  
            }
            
            base.HandleUnauthorizedRequest(filterContext);
        }
    }
}
