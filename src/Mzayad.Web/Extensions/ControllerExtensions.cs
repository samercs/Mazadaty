using Mzayad.Web.Controllers;
using OrangeJetpack.Base.Web.Utilities;
using OrangeJetpack.Services.Models;
using System;
using System.Web.Http;
using System.Web.Mvc;

namespace Mzayad.Web.Extensions
{
    internal class GenericController : Controller { }

    public static class ControllerExtensions
    {
        public static Email WithTemplate(this Email email)
        {
            var messageWithTemplate = ControllerUtilities.GetPartialViewAsString("EmailTemplate", email);
            email.Message = messageWithTemplate;

            return email;
        }
        
        [Obsolete("Deprecated in favor of parameterless WithTemplate() method.")]
        public static Email WithTemplate(this Email email, ApplicationController controller)
        {
            var messageWithTemplate = ControllerUtilities.GetPartialViewAsString("EmailTemplate", email);
            email.Message = messageWithTemplate;

            return email;
        }

        [Obsolete("Deprecated in favor of parameterless WithTemplate() method.")]
        public static Email WithTemplate(this Email email, ApiController controller)
        {
            var messageWithTemplate = ControllerUtilities.GetPartialViewAsString("EmailTemplate", email);
            email.Message = messageWithTemplate;

            return email;
        }
    }
}