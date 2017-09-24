using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mazadaty.Web.Models.WishList
{
    public class IndexViewModel
    {
        public IEnumerable<Mazadaty.Models.WishList> WishList { get; set; }
    }
}
