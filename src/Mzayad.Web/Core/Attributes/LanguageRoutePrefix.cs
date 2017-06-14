using System.Web.Mvc;

namespace Mzayad.Web.Core.Attributes
{
    public class LanguageRoutePrefix : RoutePrefixAttribute
    {
        public LanguageRoutePrefix(string prefix) : base(prefix)
        {
            
        }

        public override string Prefix
        {
            get
            {
                return "{language:regex(^en|ar$)}/" + base.Prefix;
            }
        }
    }
}
