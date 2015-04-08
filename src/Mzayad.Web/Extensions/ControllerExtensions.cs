using System;
using Mzayad.Web.Controllers;
using OrangeJetpack.Base.Web.Utilities;
using OrangeJetpack.Services.Models;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace Mzayad.Web.Extensions
{
    internal class GenericController : Controller { }

    public static class ControllerExtensions
    {
        public static Email WithTemplate(this Email email)
        {
            var messageWithTemplate = ControllerUtilities.GetPartialViewAsString(CreateController(), "EmailTemplate", email);
            email.Message = messageWithTemplate;

            return email;
        }
        
        [Obsolete("Deprecated in favor of parameterless WithTemplate() method.")]
        public static Email WithTemplate(this Email email, ApplicationController controller)
        {
            var messageWithTemplate = ControllerUtilities.GetPartialViewAsString(controller, "EmailTemplate", email);
            email.Message = messageWithTemplate;

            return email;
        }

        [Obsolete("Deprecated in favor of parameterless WithTemplate() method.")]
        public static Email WithTemplate(this Email email, ApiController controller)
        {
            var messageWithTemplate = ControllerUtilities.GetPartialViewAsString(controller, "EmailTemplate", email);
            email.Message = messageWithTemplate;

            return email;
        }

        private static GenericController CreateController()
        {
            var wrapper = new HttpContextWrapper(HttpContext.Current);

            var routeData = new RouteData();
            routeData.Values.Add("controller", "generic");
            
            var controller = new GenericController();
            controller.ControllerContext = new ControllerContext(wrapper, routeData, controller);
            
            return controller;
        }
    }
}