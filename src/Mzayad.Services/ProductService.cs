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
    public class ProductService : ServiceBase
    {
        public ProductService(IDataContextFactory dataContextFactory)
            : base(dataContextFactory)
        {
        }

        public async Task<IEnumerable<Product>> GetProducts()
        {
            using (var dc = DataContext())
            {
                var products = await dc.Products.Include(i => i.ProductImages).Include(i => i.Categories).ToListAsync();
                return products;
            }
        }

        public async Task<IEnumerable<Product>> GetProducts(string languageCode)
        {
            using (var dc = DataContext())
            {
                var products = await dc.Products.Include(i => i.ProductImages).Include(i => i.Categories).ToListAsync();
                return products.Localize(languageCode, i => i.Name, i => i.Description).OrderBy(i => i.Name);
            }
        }

        public async Task<Product> AddProduct(Product product)
        {
            using (var dc = DataContext())
            {
                dc.Products.Add(product);
                await dc.SaveChangesAsync();
                return product;

            }
        }

        public async Task<Product> UpdateProduct(Product product, List<int> categoryIds)
        {
            using (var dc = DataContext())
            {
                dc.Products.Attach(product);
                dc.SetModified(product);
                product.Categories = dc.Categories.Where(i => categoryIds.Contains(i.CategoryId)).ToList();
                await dc.SaveChangesAsync();
                return product;

            }
        }

        public async Task<Product> GetProduct(int productId)
        {
            using (var dc = DataContext())
            {
                return await dc.Products.Include(i => i.ProductImages).Include(i => i.Categories).SingleOrDefaultAsync(i => i.ProductId == productId);
            }
        }

    }
}
