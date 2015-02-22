using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mzayad.Data;
using Mzayad.Models;
using OrangeJetpack.Localization;

namespace Mzayad.Services
{
    public class CategoryService : ServiceBase
    {
        public CategoryService(IDataContextFactory dataContextFactory)
            : base(dataContextFactory)
        {
        }

        public async Task<IEnumerable<Category>> GetCategories(string languageCode="en")
        {
            using (var dc = DataContext())
            {
                var categories = await dc.Categories
                                         .Include(i => i.Children)
                                         .ToListAsync();

                return categories.Localize(languageCode, i => i.Name).OrderBy(i => i.Name);
            }
        }

        public async Task<IEnumerable<Category>> GetCategoriesAsHierarchyAsync(string languageCode = "en")
        {
            using (var dc = DataContext())
            {
                var categories = await dc.Categories
                    .OrderBy(i => i.Name)
                    .ToListAsync();

                var localizedCategories = categories.Localize(languageCode, i => i.Name).ToList();

                var parentCategories = localizedCategories
                    .Where(i => i.ParentId == null)
                    .Select(MapCategory())
                    .ToList();

                foreach (var parentCategory in parentCategories)
                {
                    SetChildren(parentCategory, localizedCategories);
                }

                return parentCategories;
            }
        }

        public async Task<Category> GetCategory(int categoryId)
        {
            using (var dc = DataContext())
            {
                return await dc.Categories
                    
                    .Include(i => i.Children)
                    
                    .SingleOrDefaultAsync(i => i.CategoryId == categoryId);
            }
        }

        private static Func<Category, Category> MapCategory()
        {
            return i => new Category
            {
                CategoryId = i.CategoryId,
                Name = i.Name,
                Slug = i.Slug,
                Parent = i.Parent,
                ParentId = i.ParentId,
                CreatedUtc = i.CreatedUtc,
                
            };
        }

        private static void SetChildren(Category parentCategory, IEnumerable<Category> categories)
        {
            categories = categories.ToList();

            parentCategory.Children = categories
                .Where(i => i.ParentId == parentCategory.CategoryId)
                .Select(MapCategory())
                .ToList();

            foreach (var category in parentCategory.Children)
            {
                SetChildren(category, categories);
            }
        }

        public async Task<Category> AddCategory(Category category)
        {
            using (var dc = DataContext())
            {

                dc.Categories.Add(category);
                await dc.SaveChangesAsync();
                return category;

            }
        }

        public async Task<Category> UpdateCategory(Category category)
        {
            using (var dc = DataContext())
            {
                dc.SetModified(category);
                await dc.SaveChangesAsync();

                return category;
            }
        }

        public void Delete(Category category)
        {
            using (var dc = DataContext())
            {

                category.Children.ToList().ForEach(i =>
                {
                    dc.Categories.Attach(i);
                    dc.Categories.Remove(i);


                });
                dc.Categories.Attach(category);
                dc.Categories.Remove(category);
                dc.SaveChanges();
            }
        }

        public async Task<Category> GetCategoryBySlug(string slug)
        {
            using (var dc = DataContext())
            {
                return await dc.Categories.SingleOrDefaultAsync(i => i.Slug == slug);
            }
        }

        public async Task<IReadOnlyCollection<string>> GetAllUrlSlugs()
        {
            using (var dc = DataContext())
            {
                return await dc.Categories.Select(i => i.Slug).OrderBy(i => i).Distinct().ToListAsync();
            }
        }
    }
}
