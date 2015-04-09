using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Mzayad.Models;
using OrangeJetpack.Localization;

namespace Mzayad.Web.Areas.admin.Models.Sponsors
{
    public class AddViewModel
    {
        public Sponsor Sponsor { get; set; }

        public AddViewModel Hydrate()
        {
            if (Sponsor == null)
            {
                Sponsor = new Sponsor()
                {
                    Name = LocalizedContent.Init()
                };
            }

            return this;
        }
    }
}