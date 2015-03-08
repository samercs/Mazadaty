using Mzayad.Models;
using Mzayad.Services;
using OrangeJetpack.Localization;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Mzayad.Web.Areas.admin.Models.Products
{
    public class EditViewModel
    {
        public Product Product { get; set; }

        public bool GoToAuction { get; set; }

        public IEnumerable<Category> Categories { get; set; }
        public IEnumerable<int> SelectedCategories { get; set; }
        public IList<EditViewProductSpecification> EditViewProductSpecifications { get; set; }
        public IEnumerable<string> SelectedSpecification { get; set; }
        

        public async Task<EditViewModel> Hydrate(ProductService productService, CategoryService categoryService,SpecificationService specificationService, Product product, string languageCode)
        {
            Product = product;
            Categories = await categoryService.GetCategoriesAsHierarchyAsync(languageCode);
            SelectedCategories = Product.Categories.Select(i => i.CategoryId).ToList();
            EditViewProductSpecifications = new List<EditViewProductSpecification>();


            var allSpecification = new List<Mzayad.Models.Specification>(await specificationService.GetAll("en"));
            foreach (var specification in product.ProductSpecifications)
            {
                EditViewProductSpecifications.Add(await new EditViewProductSpecification().Hydrate(product,specification,specificationService));
                allSpecification.Remove(
                    allSpecification.SingleOrDefault(i => i.SpecificationId == specification.SpecificationId));
            }

            foreach (var specification in allSpecification)
            {
                EditViewProductSpecifications.Add(await new EditViewProductSpecification().Hydrate(product,null,specificationService));
            }
            
            

            return this;
        }

        
    }


    public class EditViewProductSpecification
    {
        
        
        public IList<SelectListItem> SpecificationItems { get; set; }
        public ProductSpecification ProductSpecification { get; set; }

        public async Task<EditViewProductSpecification> Hydrate(Product product,ProductSpecification productSpecification,SpecificationService specificationService)
        {
            ProductSpecification = new ProductSpecification()
            {
                ProductId = product.ProductId,
                SpecificationId = productSpecification == null ? -1 : productSpecification.SpecificationId,
                Value = productSpecification==null ? LocalizedContent.Init() : productSpecification.Value
            };
            SpecificationItems=new List<SelectListItem>();
            SpecificationItems.Add(new SelectListItem()
            {
                Text = "select ...",
                Value = "",
                Selected = ProductSpecification.SpecificationId==-1
            });
            foreach (var specification in await specificationService.GetAll("en"))
            {
                SpecificationItems.Add(new SelectListItem()
                {
                    Text = specification.Name,
                    Value = specification.SpecificationId.ToString(),
                    Selected = specification.SpecificationId==ProductSpecification.SpecificationId
                });
            }


            return this;
        }
    }
}
