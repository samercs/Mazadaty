using System;
using Mzayad.Data;
using Mzayad.Models;
using OrangeJetpack.Localization;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

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
                var products = await dc.Products
                    .Where(i => i.IsDeleted == false)
                    .Include(i => i.ProductImages)
                    .Include(i => i.Categories).ToListAsync();
                return products;
            }
        }

        public async Task<IEnumerable<Product>> GetProducts(string languageCode)
        {
            using (var dc = DataContext())
            {
                var products = await dc.Products
                    .Where(i => i.IsDeleted == false)
                    .Include(i => i.ProductImages)
                    .Include(i => i.Categories)
                    .ToListAsync();

                return products.Localize(languageCode, i => i.Name, i => i.Description).OrderBy(i => i.Name);
            }
        }

        /// <summary>
        /// Gets a unique list of all product names in both English and Arabic.
        /// </summary>
        public async Task<IEnumerable<string>> GetProductNames()
        {
            using (var dc = DataContext())
            {
                var names = await dc.Products
                    .Where(i => i.IsDeleted == false)
                    .Select(i => i.Name).ToListAsync();

                var products = names.Select(i => new Product { Name = i }).ToList();

                var en = products.Localize("en", i => i.Name).Select(i => i.Name);
                var ar = products.Localize("ar", i => i.Name).Select(i => i.Name);

                return Enumerable.Concat(en, ar).Distinct().OrderBy(i => i);
            }
        }

        public async Task<IEnumerable<Product>> GetProductsWithoutCategory(string languageCode, string search = null)
        {
            using (var dc = DataContext())
            {
                var query = dc.Products.Where(i => i.IsDeleted == false);

                if (!string.IsNullOrEmpty(search))
                {
                    query = query.Where(i => i.Name.Contains(search) || i.Description.Contains(search));
                }

                var products = await query.ToListAsync();

                return products.Localize(languageCode, i => i.Name, i => i.Description)
                    .OrderBy(i => i.Name);
            }
        }

        public async Task<Product> AddProduct(Product product)
        {
            using (var dc = DataContext())
            {
                product.Quantity = 1;

                dc.Products.Add(product);
                await dc.SaveChangesAsync();
                return product;
            }
        }

        public async Task<Product> UpdateProduct(Product product, IList<int> categoryIds, List<ProductSpecification> productSpecifications)
        {
            using (var dc = DataContext())
            {
                dc.Products.Attach(product);
                dc.SetModified();

                if (categoryIds != null && categoryIds.Any())
                {
                    product.Categories = dc.Categories.Where(i => categoryIds.Contains(i.CategoryId)).ToList();
                }

                product.ProductSpecifications = productSpecifications;
                await dc.SaveChangesAsync();
                return product;

            }
        }

        public async Task<Product> GetProduct(int productId)
        {
            using (var dc = DataContext())
            {
                var product = await dc.Products
                    .Where(i => i.IsDeleted == false)
                    .Include(i => i.ProductImages)
                    .Include(i => i.Categories)
                    .Include(i => i.ProductSpecifications.Select(j => j.Specification))
                    .Include(i => i.Sponsor)
                    .SingleOrDefaultAsync(i => i.ProductId == productId);

                if (product == null)
                {
                    return null;
                }

                product.ProductImages = product.ProductImages.OrderBy(i => i.SortOrder).ToList();

                return product;
            }
        }

        public async Task<IReadOnlyCollection<Product>> GetProductsByDate(DateTime startDate, DateTime endDate, string language = "en")
        {
            using (var dc = DataContext())
            {
                var products = await dc.Products
                    .Where(i => i.IsDeleted == false)
                    .Where(i => i.CreatedUtc >= startDate)
                    .Where(i => i.CreatedUtc <= endDate)
                    .ToListAsync();

                return products.Localize(language, i => i.Name).ToList();
            }
        }

        public async Task<ProductImage> AddProductImage(Product product, string imageSmUrl, string imageMdUrl, string imageLgUrl)
        {
            using (var dc = DataContext())
            {
                if (ProductImageHaveDuplicate(product))
                {
                    await ProductImageFixDuplicate(product);
                }

                var productImage = new ProductImage
                {
                    SortOrder = product.ProductImages.Count > 0 ? (product.ProductImages.Last().SortOrder + 1) : 1,
                    ImageSmUrl = imageSmUrl,
                    ImageMdUrl = imageMdUrl,
                    ImageLgUrl = imageLgUrl,
                    ProductId = product.ProductId
                };

                dc.ProductImages.Add(productImage);

                await dc.SaveChangesAsync();

                return productImage;
            }
        }

        public void DeleteProductImage(ProductImage productImage)
        {
            using (var dc = DataContext())
            {
                dc.ProductImages.Attach(productImage);
                dc.ProductImages.Remove(productImage);
                dc.SaveChanges();
            }
        }

        public bool ProductImageHaveDuplicate(Product product)
        {
            var d = new HashSet<double>();
            foreach (var t in product.ProductImages)
            {
                if (!d.Add(t.SortOrder))
                {
                    return true;
                }
            }
            return false;
        }

        public async Task<bool> ProductImageFixDuplicate(Product product)
        {
            using (var dc = DataContext())
            {
                int count = 1;
                foreach (var t in product.ProductImages.OrderBy(i => i.CreatedUtc))
                {
                    t.SortOrder = count;
                    dc.SetModified();
                    ++count;
                }
                await dc.SaveChangesAsync();

                return true;
            }



        }

        public async Task<ProductImage> GetProductImage(int id)
        {
            using (var dc = DataContext())
            {
                return await dc.ProductImages.SingleOrDefaultAsync(i => i.ProductImageId == id);
            }
        }

        public async Task<bool> UpdateProductImageOrder(int imageId, int newIndex)
        {
            using (var dc = DataContext())
            {
                var image = await dc.ProductImages.SingleOrDefaultAsync(i => i.ProductImageId == imageId);
                if (image != null)
                {
                    var sku =
                        await dc.ProductImages.OrderBy(i => i.SortOrder).Where(i => i.ProductId == image.ProductId).ToListAsync();
                    if (sku.Count > 1 && newIndex < sku.Count)
                    {
                        double index;
                        //move to first image
                        if (newIndex == 0)
                        {
                            index = sku[0].SortOrder / 2;
                        }
                        //move to last image
                        else if ((newIndex + 1) == sku.Count)
                        {
                            index = sku[sku.Count - 1].SortOrder + 1;
                        }
                        //move between two image
                        else
                        {
                            index = (sku[newIndex].SortOrder + sku[newIndex - 1].SortOrder) / 2;
                        }

                        image.SortOrder = index;
                        dc.SetModified();
                        await dc.SaveChangesAsync();
                    }

                }
                return true;
            }
        }

        public async Task DeleteProduct(int productId)
        {
            using (var dc = DataContext())
            {
                var product = await dc.Products.SingleOrDefaultAsync(i => i.ProductId == productId);
                if (product == null)
                {
                    return;
                }

                product.IsDeleted = true;
                product.DeletedUtc = DateTime.UtcNow;
                await dc.SaveChangesAsync();
            }
        }

    }
}
