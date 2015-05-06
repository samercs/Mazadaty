using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kendo.Mvc.Extensions;
using Mzayad.Models;
using Mzayad.Services;

namespace Mzayad.Web.Areas.admin.Models.WishList
{
    public class IndexViewModel
    {
        public IEnumerable<WishListAdminModel> WishLists { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public async Task<IndexViewModel> Hydrate(WishListService wishListService,DateTime? startDate,DateTime? endDate)
        {
            StartDate = StartDate ?? DateTime.Today.AddYears(-1);
            EndDate = EndDate ?? DateTime.Today;
            WishLists =(await wishListService.GetGroupBy(startDate,endDate));

            return this;
        }
    }

    
}
