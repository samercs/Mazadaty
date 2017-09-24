using Mazadaty.Web.Controllers;
using OrangeJetpack.Base.Web.Utilities;
using System;
using System.Web.Http;
using System.Web.Mvc;
using Mazadaty.Services.Messaging;

namespace Mazadaty.Web.Extensions
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
        
        
    }
}
