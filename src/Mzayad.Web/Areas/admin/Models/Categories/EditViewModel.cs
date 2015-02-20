﻿using System;
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
        public IEnumerable<SelectListItem> PotentialParentCategories { get; set; }

        public async Task<EditViewModel> Hydrate(Category category, CategoryService categoryService)
        {
            Category = category;
            ParentId = Category.ParentId.HasValue ? Category.ParentId.Value : (int?)null;

            var categories = (await categoryService.GetCategories("en")).ToList();

            PotentialParentCategories = categories
                .Where(i => i.CategoryId != Category.CategoryId)
                .Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.CategoryId.ToString(CultureInfo.InvariantCulture)
                })
                .OrderBy(i => i.Text);

           
            return this;
        }
    }
}