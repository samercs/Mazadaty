using System.Collections.Generic;
using Mazadaty.Models;

namespace Mazadaty.Web.Models.Home
{
    public class CategoryListModel
    {
        public IReadOnlyCollection<Category> Categories { get; set; }
        public int? CategoryId { get; set; }
    }
}
