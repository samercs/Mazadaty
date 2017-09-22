using Mzayad.Web.Controllers;
using OrangeJetpack.Base.Web.Utilities;
using System;
using System.Web.Http;
using System.Web.Mvc;
using Mzayad.Services.Messaging;

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
        
        
    }
}