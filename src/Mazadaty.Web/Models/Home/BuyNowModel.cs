using System.Collections.Generic;
using Mazadaty.Models;

namespace Mazadaty.Web.Models.Home
{
    public class BuyNowModel
    {
        public string Search { get; set; }
        public int? CategoryId { get; set; }
        public IReadOnlyCollection<Auction> Auctions { get; set; }
        public IReadOnlyCollection<Category> Categories { get; set; }

        public CategoryListModel CategoryList => new CategoryListModel
        {
            Categories = Categories,
            CategoryId = CategoryId
        };
    }
}
