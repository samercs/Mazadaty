using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Mazadaty.Models;

namespace Mazadaty.Web.Areas.admin.Models.Products
{
    public class IndexViewModel
    {
        public string Search { get; set; }
        public IEnumerable<Product> Products { get; set; }
    }
}
