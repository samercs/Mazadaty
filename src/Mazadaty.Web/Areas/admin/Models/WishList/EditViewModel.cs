using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mazadaty.Services;

namespace Mazadaty.Web.Areas.admin.Models.WishList
{
    public class EditViewModel
    {
        public IEnumerable<Mazadaty.Models.WishList> WishLists { get; set; }
        [Required]
        public string NameNormalized { get; set; }

        public async Task<EditViewModel> Hydrate(WishListService wishListService,string nameNormalized)
        {
            this.NameNormalized = nameNormalized;
            WishLists = await wishListService.GetByNameNormalized(NameNormalized);
            return this;
        }
    }
}
