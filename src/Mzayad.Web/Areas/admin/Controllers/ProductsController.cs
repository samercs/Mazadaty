using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Mzayad.Models;
using Mzayad.Services;
using Mzayad.Web.Areas.admin.Models.Products;
using Mzayad.Web.Controllers;
using Mzayad.Web.Core.Attributes;
using Mzayad.Web.Core.Configuration;
using Mzayad.Web.Core.Identity;
using Mzayad.Web.Core.Services;
using OrangeJetpack.Base.Core.Formatting;
using OrangeJetpack.Localization;
using OrangeJetpack.Services.Client.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Mzayad.Web.Areas.admin.Controllers
{
    [RouteArea("Admin"), RoutePrefix("products"), RoleAuthorize(Role.Administrator)]
    public class ProductsController : ApplicationController
    {
        private readonly ProductService _productService;
        private readonly CategoryService _categoryService;
        private readonly IStorageService _storageService;
        private readonly SpecificationService _specificationService;

        public ProductsController(IControllerServices controllerServices,IStorageService storageService) : base(controllerServices)
        {
            _productService=new ProductService(controllerServices.DataContextFactory);
            _categoryService=new CategoryService(controllerServices.DataContextFactory);
            _storageService = storageService;
            _specificationService=new SpecificationService(DataContextFactory);
        }

        public async Task<ActionResult> Index(string search="")
        {
            var products = await _productService.GetProductsWithoutCategory("en",search);
            foreach (var product in products)
            {
                product.Description = StringFormatter.StripHtmlTags(product.Description);
            }

            var model = new IndexViewModel()
            {
                Search = "",
                Products = products
            };


            return View(model);
        }


        public async Task<JsonResult> GetProducts([DataSourceRequest] DataSourceRequest request,string search=null)
        {
            var products = await _productService.GetProductsWithoutCategory("en",search);
            foreach (var product in products)
            {
                product.Description = StringFormatter.StripHtmlTags(product.Description);
            }
            return Json(products.ToDataSourceResult(request));
        }
            
        [Route("add")]
        public ActionResult Add(bool goToAuction=false)
        {
            var model = new AddViewModel().Hydrate(_productService);
            model.Product.CreatedByUserId =  AuthService.CurrentUserId();
            model.GoToAuction = goToAuction;
            return View(model);
        }

        [Route("add")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Add(AddViewModel model, LocalizedContent[] name)
        {
            model.Product.Name = name.Serialize();
            model.Product.CreatedByUserId = AuthService.CurrentUserId();

            if (!ModelState.IsValid)
            {
                return AddErrorView(model);
            }

            var product = await _productService.AddProduct(model.Product);
            
            var productName = product.Localize("en", i => i.Name).Name;

            SetStatusMessage(string.Format("Product {0} successfully added.", productName));

            return RedirectToAction("Edit", new { id = product.ProductId , goToAuction=model.GoToAuction  });
        }

        public ActionResult AddErrorView(AddViewModel model, string modelStateKey = null, string errorMessage = null)
        {
            if (modelStateKey != null && errorMessage != null)
            {
                ModelState.AddModelError(modelStateKey, errorMessage);
            }

            model = model.Hydrate(_productService);

            return View("Add", model);
        }

        [Route("edit/{id:int}")]
        public async Task<ActionResult> Edit(int id,bool goToAuction=false)
        {
            var product = await _productService.GetProduct(id);
            if (product == null)
            {
                return HttpNotFound();
            }

            var model = await new EditViewModel().Hydrate(_productService, _categoryService,_specificationService, product, "en");
            model.GoToAuction = goToAuction;
            return View(model);
        }

        [Route("edit/{id:int}")]
        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
        public async Task<ActionResult> Edit(int id, EditViewModel model, LocalizedContent[] name, LocalizedContent[] description,FormCollection data)
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
            product.VideoUrl = model.Product.VideoUrl;

            var specificationsContent = GetSpecificationsLocalizedContent(data);
            var productSpecifications = GetProductSpecifications(model, product, specificationsContent);

            await _productService.UpdateProduct(product, model.SelectedCategories, productSpecifications);

            var productName = product.Localize("en", i => i.Name).Name;

            SetStatusMessage(string.Format("Product {0} successfully updated.", productName));

            return model.GoToAuction 
                ? RedirectToAction("Create", "Auctions", new { product.ProductId }) 
                : RedirectToAction("Index");
        }

        private static List<ProductSpecification> GetProductSpecifications(EditViewModel model, Product product, List<LocalizedContent[]> values)
        {
            var x = 0;
            var productSpecificationList = model.SelectedSpecification.Where(str => int.TryParse(str, out x)).Select(str => x).ToList();
            var productSpecifications = new List<ProductSpecification>();

            for (int i = 0; i < productSpecificationList.Count; i++)
            {
                if (productSpecificationList[i] != -1)
                {
                    var productSpecification = new ProductSpecification()
                    {
                        ProductId = product.ProductId,
                        SpecificationId = productSpecificationList[i],
                        Value = values[i].Serialize()
                    };
                    productSpecifications.Add(productSpecification);
                }
            }
            return productSpecifications;
        }

        private static List<LocalizedContent[]> GetSpecificationsLocalizedContent(FormCollection data)
        {
            var enValue = new List<string>();
            var arValue = new List<string>();

            foreach (var key in data.AllKeys)
            {
                if (key.Equals("Value[0].Value"))
                {
                    var tmp = data[key].Split(new char[] {','}, StringSplitOptions.None);
                    enValue.AddRange(tmp);
                }
                else if (key.Equals("Value[1].Value"))
                {
                    var tmp = data[key].Split(new char[] {','}, StringSplitOptions.None);
                    arValue.AddRange(tmp);
                }
            }

            var values = new List<LocalizedContent[]>();
            for (int i = 0; i < enValue.Count; i++)
            {
                var en = new LocalizedContent()
                {
                    Key = "en",
                    Value = enValue[i]
                };
                var ar = new LocalizedContent()
                {
                    Key = "ar",
                    Value = arValue[i]
                };

                values.Add(new[] {en, ar});
            }
            return values;
        }

        private async Task<Uri[]> UploadImage(HttpPostedFileBase file, int[] widths)
        {
            if (file == null || file.ContentLength == 0 || !file.ContentType.Contains("image"))
            {
                throw new ArgumentException("Please only upload an image.");
            }

            var uris = await _storageService.SaveImage("products", file, widths);
            if (uris.Length != widths.Length)
            {
                throw new Exception("Could not upload image, image service did not return expected results.");
            }

            return uris;
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<JsonResult> UploadImage(HttpPostedFileBase file, int parentId, int? itemId)
        {
            if (itemId != null)
            {
                await DeleteImage(itemId.Value);
            }

            try
            {
                var product = await _productService.GetProduct(parentId);
                if (product == null)
                {
                    throw new Exception("Could not upload image. product not available.");
                }

                var widths = new[] { ImageWidthConstants.ProductImageSm, ImageWidthConstants.ProductImageMd, ImageWidthConstants.ProductImageLg };
                var uris = await UploadImage(file, widths);

                var imageSmUrl = uris[0];
                var imageMdUrl = uris[1];
                var imageLgUrl = uris[2];

                var productImage = await _productService.AddProductImage(product, imageSmUrl.AbsoluteUri, imageMdUrl.AbsoluteUri, imageLgUrl.AbsoluteUri);

                return Json(new
                {
                    itemId = productImage.ProductImageId,
                    url = imageMdUrl.AbsoluteUri
                });
            }
            catch (Exception ex)
            {
                
                return JsonError("Could not upload image.");
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<JsonResult> DeleteImage(int itemId)
        {
            try
            {
                var productImage = await _productService.GetProductImage(itemId);
                if (productImage != null)
                {
                    var productImages = new[] { productImage.ImageSmUrl, productImage.ImageMdUrl, productImage.ImageLgUrl };
                    var fileNames = productImages.Select(Path.GetFileName);

                    foreach (var fileName in fileNames.Where(i => !string.IsNullOrWhiteSpace(i)))
                    {
                        await _storageService.DeleteFile("products", fileName);
                    }

                    _productService.DeleteProductImage(productImage);
                }

                return Json(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return JsonError("Could not delete image.");
            }
        }

        public async Task<ActionResult> UpdateImageOrder(int imageId, int newIndex)
        {
            await _productService.UpdateProductImageOrder(imageId, newIndex);
            return Content("Done");
        }

	}
}