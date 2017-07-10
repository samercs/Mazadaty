using System.Collections.Generic;
using Mzayad.Models;

namespace Mzayad.Web.Models.Home
{
    public class CategoryListModel
    {
        public IReadOnlyCollection<Category> Categories { get; set; }
        public int? CategoryId { get; set; }
    }
}