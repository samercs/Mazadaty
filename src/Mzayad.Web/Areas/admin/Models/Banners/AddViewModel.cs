using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Humanizer;
using Mzayad.Models;
using Mzayad.Models.Enums;

namespace Mzayad.Web.Areas.Admin.Models.Banners
{
    public class AddViewModel
    {
        public Banner Banner { get; set; }

        public IEnumerable<SelectListItem> StatusList
        {
            get {
                return Enum.GetValues(typeof(BannerStatus)).Cast<BannerStatus>().Select(i => new SelectListItem
                {
                    Text = i.Humanize(),
                    Value = i.ToString(),
                    Selected = Banner.Status == i
                });
            }
        }
    }
}