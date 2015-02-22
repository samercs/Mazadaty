using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Mzayad.Models;
using Mzayad.Services;
using OrangeJetpack.Localization;

namespace Mzayad.Web.Areas.admin.Models.Categories
{
    public class AddViewModel
    {
        public Category Category { get; set; }

        public HttpPostedFileBase Photo { get; set; }
        public IEnumerable<SelectListItem> Categories { get; set; }
        //public IEnumerable<string> ExistingSlugs { get; set; } 

        public async Task<AddViewModel> Hydrate(CategoryService categoryService)
        {
            if (Category == null)
            {
                Category = new Category
                {
                    Name = LocalizedContent.Init()
                };
            }

            var categories = (await categoryService.GetCategories()).ToList();

            Categories = categories.Select(i => new SelectListItem
            {
                Value = i.CategoryId.ToString(),
                Text = i.Name,
                Selected = i.CategoryId.Equals(Category.ParentId)
            });

            

            return this;
        }
    }
}