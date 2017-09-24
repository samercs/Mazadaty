using OrangeJetpack.Localization;

namespace Mazadaty.Web.Areas.admin.Models.Specification
{
    public class AddViewModel
    {
        public Mazadaty.Models.Specification Specification { get; set; }

        public AddViewModel Hydrate()
        {
            if (Specification == null)
            {
                Specification = new Mazadaty.Models.Specification()
                {
                    Name = LocalizedContent.Init()
                };
            }

            return this;
        }
    }
}
