using Mzayad.Models;
using Mzayad.Services;
using OrangeJetpack.Localization;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Mzayad.Web.Areas.admin.Models.Categories
{
    public class AddViewModel
    {
        public Category Category { get; set; }
        public IEnumerable<SelectListItem> Categories { get; set; }
        public IEnumerable<string> ExistingSlugs { get; set; } 

        public async Task<AddViewModel> Hydrate(CategoryService categoryService)
        {
            if (Category == null)
            {
                Category = new Category
                {
                    Name = LocalizedContent.Init()
                };
            }

            var categories = await categoryService.GetCategories();

            Categories = categories.Select(i => new SelectListItem
            {
                Value = i.CategoryId.ToString(),
                Text = i.Name,
                Selected = i.CategoryId.Equals(Category.ParentId)
            });

            ExistingSlugs = await categoryService.GetAllUrlSlugs();

            return this;
        }
    }
}