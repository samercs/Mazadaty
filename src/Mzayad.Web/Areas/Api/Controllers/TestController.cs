using System.Web.Http;

namespace Mzayad.Web.Areas.Api.Controllers
{
    public class TestController : ApiController
    {
        public IHttpActionResult Get()
        {
            return Ok("asdf");
        }
    }
}
