using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Mazadaty.Models;
using OrangeJetpack.Localization;

namespace Mazadaty.Web.Areas.admin.Models.Sponsors
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
