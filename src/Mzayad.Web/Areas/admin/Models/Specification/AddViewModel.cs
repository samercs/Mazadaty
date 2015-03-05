using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using OrangeJetpack.Localization;

namespace Mzayad.Web.Areas.admin.Models.Specification
{
    public class AddViewModel
    {
        public Mzayad.Models.Specification Specification { get; set; }

        public async Task<AddViewModel> Hydrate()
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