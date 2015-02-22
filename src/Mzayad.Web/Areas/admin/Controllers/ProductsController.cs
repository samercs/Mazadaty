using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Mzayad.Models;
using Mzayad.Services;
using Mzayad.Web.Areas.admin.Models.Products;
using Mzayad.Web.Controllers;
using Mzayad.Web.Core.Services;
using OrangeJetpack.Base.Core.Formatting;
using OrangeJetpack.Localization;

namespace Mzayad.Web.Areas.admin.Controllers
{
    [RouteArea("Admin"), RoutePrefix("products")]
    public class ProductsController : ApplicationController
    {
        private readonly ProductService _productService;
        private readonly CategoryService _categoryService;
        //
        // GET: /admin/Products/
        public ProductsController(IControllerServices controllerServices) : base(controllerServices)
        {
            _productService=new ProductService(controllerServices.DataContextFactory);
            _categoryService=new CategoryService(controllerServices.DataContextFactory);
        }

        public async Task<ActionResult> Index()
        {
            var products = await _productService.GetProducts("en");
            foreach (var product in products)
            {
                product.Description = StringFormatter.StripHtmlTags(product.Description);
            }

            return View(products);
        }

        [Route("add")]
        public async Task<ActionResult> Add()
        {
            var model = await new AddViewModel().Hydrate(_productService);

            return View(model);
        }

        [Route("add")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Add(AddViewModel model, LocalizedContent[] name)
        {
            model.Product.Name = name.Serialize();

            if (!ModelState.IsValid)
            {
                return await AddErrorView(model);
            }

            Product product = null;
            
            product = await _productService.AddProduct(model.Product);
            
            var productName = product.Localize("en", i => i.Name).Name;

            SetStatusMessage(string.Format("Product {0} successfully added.", productName));

            return RedirectToAction("Edit", new { id = product.ProductId });
        }

        public async Task<ActionResult> AddErrorView(AddViewModel model, string modelStateKey = null, string errorMessage = null)
        {
            if (modelStateKey != null && errorMessage != null)
            {
                ModelState.AddModelError(modelStateKey, errorMessage);
            }

            model = await model.Hydrate(_productService);

            return View("Add", model);
        }

        [Route("edit/{id:int}")]
        public async Task<ActionResult> Edit(int id)
        {
            var product = await _productService.GetProduct(id);
            if (product == null)
            {
                return HttpNotFound();
            }

            var model = await new EditViewModel().Hydrate(_productService, _categoryService, product, "en");

            return View(model);
        }

        [Route("edit/{id:int}")]
        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
        public async Task<ActionResult> Edit(int id, EditViewModel model, LocalizedContent[] name, LocalizedContent[] description)
        {
            var product = await _productService.GetProduct(id);
            if (product == null)
            {
                return HttpNotFound();
            }

            model.Product.Name = name.Serialize();

            foreach (var localizedContent in description)
            {
                localizedContent.Value = StringFormatter.StripWordHtml(localizedContent.Value);
            }

            model.Product.Description = description.Serialize();

            

            if (!TryUpdateModel(product, "Product"))
            {
                return View(model);
            }

            product.Name = model.Product.Name;
            product.Description = model.Product.Description;
            product.RetailPrice = model.Product.RetailPrice;
            var x = 0;
            var categoryIds = model.SelectedCategories.Where(str => int.TryParse(str, out x)).Select(str => x).ToList();

            await _productService.UpdateProduct(product, categoryIds);
            

            var productName = product.Localize("en", i => i.Name).Name;

            SetStatusMessage(string.Format("Product {0} successfully updated.", productName));
            
            return RedirectToAction("Index");
        }

	}
}