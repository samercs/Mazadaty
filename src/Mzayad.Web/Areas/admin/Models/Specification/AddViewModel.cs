using OrangeJetpack.Localization;

namespace Mzayad.Web.Areas.admin.Models.Specification
{
    public class AddViewModel
    {
        public Mzayad.Models.Specification Specification { get; set; }

        public AddViewModel Hydrate()
        {
            if (Specification == null)
            {
                Specification = new Mzayad.Models.Specification()
                {
                    Name = LocalizedContent.Init()
                };
            }

            return this;
        }
    }
}