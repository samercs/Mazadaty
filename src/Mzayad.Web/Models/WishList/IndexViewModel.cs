using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mzayad.Web.Models.WishList
{
    public class IndexViewModel
    {
        public IEnumerable<Mzayad.Models.WishList> WishList { get; set; }
    }
}