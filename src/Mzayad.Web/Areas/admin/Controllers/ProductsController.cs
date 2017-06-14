using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Mzayad.Core.Formatting;
using Mzayad.Models;
using Mzayad.Services;
using Mzayad.Web.Areas.admin.Models.Products;
using Mzayad.Web.Controllers;
using Mzayad.Web.Core.Attributes;
using Mzayad.Web.Core.Configuration;
using Mzayad.Web.Core.Identity;
using Mzayad.Web.Core.Services;
using OrangeJetpack.Base.Core.Formatting;
using OrangeJetpack.Base.Web;
using OrangeJetpack.Localization;
using OrangeJetpack.Services.Client.Storage;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Mzayad.Web.Areas.admin.Controllers
{
    [RouteArea("admin"), RoutePrefix("products"), RoleAuthorize(Role.Administrator)]
    public class ProductsController : ApplicationController
    {
        private readonly ProductService _productService;
        private readonly CategoryService _categoryService;
        private readonly IStorageService _storageService;
        private readonly SpecificationService _specificationService;
        private readonly SponsorService _sponsorService;

        public ProductsController(IAppServices appServices, IStorageService storageService) : base(appServices)
        {
            _productService = new ProductService(appServices.DataContextFactory);
            _categoryService = new CategoryService(appServices.DataContextFactory);
            _storageService = storageService;
            _specificationService = new SpecificationService(DataContextFactory);
            _sponsorService = new SponsorService(appServices.DataContextFactory);
        }

        public async Task<ActionResult> Index(string search = "")
        {
            var products = (await _productService.GetProductsWithoutCategory("en", search)).ToList();
            foreach (var product in products)
            {
                product.Description = StringFormatter.StripHtmlTags(product.Description);
            }

            var model = new IndexViewModel
            {
                Search = "",
                Products = products
            };

            return View(model);
        }

        public async Task<JsonResult> GetProducts([DataSourceRequest] DataSourceRequest request, string search = null)
        {
            var products = (await _productService.GetProductsWithoutCategory("en", search)).ToList();
            foreach (var product in products)
            {
                product.Description = StringFormatter.StripHtmlTags(product.Description);
            }
            return Json(products.ToDataSourceResult(request));
        }

        [Route("add")]
        public ActionResult Add(bool goToAuction = false)
        {
            var model = new AddViewModel().Hydrate(_productService);
            model.Product.CreatedByUserId = AuthService.CurrentUserId();
            model.GoToAuction = goToAuction;
            return View(model);
        }

        [Route("add")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Add(AddViewModel model, LocalizedContent[] name)
        {
            model.Product.Name = name.Serialize();
            model.Product.CreatedByUserId = AuthService.CurrentUserId();
            ModelState["Product.Name"].Errors.Clear();

            if (!ModelState.IsValid)
            {
                return AddErrorView(model);
            }

            var product = await _productService.AddProduct(model.Product);

            var productName = product.Localize("en", i => i.Name).Name;

            SetStatusMessage($"Product {productName} successfully added.");

            return RedirectToAction("Edit", new { id = product.ProductId, goToAuction = model.GoToAuction });
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
        public async Task<ActionResult> Edit(int id, bool goToAuction = false)
        {
            var product = await _productService.GetProduct(id);
            if (product == null)
            {
                return HttpNotFound();
            }

            var model = await new EditViewModel().Hydrate(_productService, _categoryService, _specificationService, _sponsorService, product, "en");
            model.GoToAuction = goToAuction;
            return View(model);
        }

        [Route("edit/{id:int}")]
        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
        public async Task<ActionResult> Edit(int id, EditViewModel model, LocalizedContent[] name, LocalizedContent[] description, FormCollection data)
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
            ModelState["Product.Name"].Errors.Clear();
            ModelState["Product.Description"].Errors.Clear();

            if (!TryUpdateModel(product, "Product"))
            {
                model = await model.Hydrate(_productService, _categoryService, _specificationService, _sponsorService, product,
                    Language);
                return View(model);
            }

            product.Name = model.Product.Name;
            product.Description = model.Product.Description;
            product.RetailPrice = model.Product.RetailPrice;
            product.VideoUrl = model.Product.VideoUrl;
            product.Notes = model.Product.Notes;
            product.SponsorId = model.Product.SponsorId;
            product.WaiveShippingCost = model.Product.WaiveShippingCost;

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
            if (model.SelectedSpecification == null)
            {
                return null;
            }
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
                    var tmp = data[key].Split(new[] { ',' }, StringSplitOptions.None);
                    enValue.AddRange(tmp);
                }
                else if (key.Equals("Value[1].Value"))
                {
                    var tmp = data[key].Split(new[] { ',' }, StringSplitOptions.None);
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

                values.Add(new[] { en, ar });
            }
            return values;
        }

        private async Task<Uri[]> UploadImage(HttpPostedFileBase file, int[] widths)
        {
            if (file == null || file.ContentLength == 0 || !file.ContentType.Contains("image"))
            {
                throw new ArgumentException("Please only upload an image.");
            }

            var imageSettings = new ImageSettings
            {
                BackgroundColor = Color.White,
                ForceSquare = true,
                Widths = widths
            };

            var uris = await _storageService.SaveImage("products", file, imageSettings);
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

                var imageSmUrl = UrlFormatter.GetCdnUrl(uris[0]);
                var imageMdUrl = UrlFormatter.GetCdnUrl(uris[1]);
                var imageLgUrl = UrlFormatter.GetCdnUrl(uris[2]);

                var productImage = await _productService.AddProductImage(product, imageSmUrl, imageMdUrl, imageLgUrl);

                return Json(new
                {
                    itemId = productImage.ProductImageId,
                    url = imageMdUrl
                });
            }
            catch (Exception)
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
            catch (Exception)
            {
                return JsonError("Could not delete image.");
            }
        }

        public async Task<ActionResult> UpdateImageOrder(int imageId, int newIndex)
        {
            await _productService.UpdateProductImageOrder(imageId, newIndex);
            return Content("Done");
        }

        [Route("delete/{productId:int}")]
        public async Task<ActionResult> Delete(int productId)
        {
            var product = await _productService.GetProduct(productId);
            if (product == null)
            {
                SetStatusMessage("Sorry this product not found", StatusMessageType.Warning);
                return RedirectToAction("Index", "Products");
            }

            return DeleteConfirmation("Delete Product", "Are you sure you want to permanently delete this product?");
        }

        [Route("delete/{productId:int}")]
        [HttpPost]
        public async Task<ActionResult> DeleteProduct(int productId)
        {
            await _productService.DeleteProduct(productId);
            SetStatusMessage("Product was successfully deleted");
            return RedirectToAction("Index", "Products");
        }
    }
}