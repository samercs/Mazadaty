using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Mzayad.Web.Areas.Api.Controllers;

namespace Mzayad.Web.Areas.Api.Filters
{
    public class LanguageFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext filterContext)
        {
            var controller = filterContext.ControllerContext.Controller as ApplicationApiController;
            if (controller == null)
            {
                return;
            }

            var language = "en";
            IEnumerable<string> headerValues;
            if (filterContext.Request.Headers.TryGetValues("accept-language", out headerValues))
            {
                language = headerValues.FirstOrDefault();
            }

            controller.Language = language;
        }
    }
}