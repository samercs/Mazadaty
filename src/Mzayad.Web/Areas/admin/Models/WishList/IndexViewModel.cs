using Mzayad.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Web;
using Mzayad.Models;

namespace Mzayad.Web.Areas.admin.Models.WishList
{
    public class IndexViewModel
    {
        public IEnumerable<WishListAdminModel> WishLists { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)] 
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public async Task<IndexViewModel> Hydrate(WishListService wishListService,DateTime? startDate,DateTime? endDate)
        {
            StartDate = startDate ?? DateTime.Today.AddYears(-1);
            EndDate = endDate ?? DateTime.Today;
            WishLists =(await wishListService.GetGroupBy(startDate,endDate));

            foreach (var wishlist in WishLists)
            {
                wishlist.NameUrlEscaped = HttpUtility.UrlEncode(wishlist.Name);
            }

            return this;
        }
    }
}
