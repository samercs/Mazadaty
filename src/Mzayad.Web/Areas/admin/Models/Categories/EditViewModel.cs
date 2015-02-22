using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Mzayad.Models;
using Mzayad.Services;

namespace Mzayad.Web.Areas.admin.Models.Categories
{
    public class EditViewModel
    {
        public Category Category { get; set; }
        public int? ParentId { get; set; }
        public IEnumerable<SelectListItem> Categories { get; set; }
        public IEnumerable<string> ExistingSlugs { get; set; }

        public async Task<EditViewModel> Hydrate(Category category, CategoryService categoryService)
        {
            Category = category;
            ParentId = Category.ParentId;

            var categories = await categoryService.GetCategories("en");

            Categories = categories
                .Where(i => i.CategoryId != Category.CategoryId)
                .Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.CategoryId.ToString(CultureInfo.InvariantCulture)
                })
                .OrderBy(i => i.Text);

            ExistingSlugs = (await categoryService.GetAllUrlSlugs()).Where(i => !i.Equals(category.Slug));

            return this;
        }
    }
}