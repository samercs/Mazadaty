using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Mzayad.Models;
using Mzayad.Services;

namespace Mzayad.Web.Areas.admin.Models.Products
{
    public class EditViewModel
    {
        public Product Product { get; set; }
        
        

        public IEnumerable<Category> Categories { get; set; }
        public IEnumerable<string> SelectedCategories { get; set; }
        
        public async Task<EditViewModel> Hydrate(ProductService productService, CategoryService categoryService, Product product, string languageCode)
        {
            Product = product;
            Categories = await categoryService.GetCategoriesAsHierarchyAsync(languageCode);
            SelectedCategories = Product.Categories.Select(i => i.CategoryId.ToString()).ToList();
            
            return this;
        }

        
    }
}
