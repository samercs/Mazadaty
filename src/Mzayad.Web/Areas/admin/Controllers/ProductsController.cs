﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Mzayad.Models;
using Mzayad.Services;
using Mzayad.Web.Areas.admin.Models.Products;
using Mzayad.Web.Controllers;
using Mzayad.Web.Core.Configuration;
using Mzayad.Web.Core.Services;
using OrangeJetpack.Base.Core.Formatting;
using OrangeJetpack.Localization;
using OrangeJetpack.Services.Client.Storage;

namespace Mzayad.Web.Areas.admin.Controllers
{
    [RouteArea("Admin"), RoutePrefix("products")]
    public class ProductsController : ApplicationController
    {
        private readonly ProductService _productService;
        private readonly CategoryService _categoryService;
        private readonly IStorageService _storageService;
        //
        // GET: /admin/Products/
        public ProductsController(IControllerServices controllerServices,IStorageService storageService) : base(controllerServices)
        {
            _productService=new ProductService(controllerServices.DataContextFactory);
            _categoryService=new CategoryService(controllerServices.DataContextFactory);
            _storageService = storageService;
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
            product.VideoUrl = model.Product.VideoUrl;
            
            var x = 0;
            var categoryIds = model.SelectedCategories.Where(str => int.TryParse(str, out x)).Select(str => x).ToList();

            await _productService.UpdateProduct(product, categoryIds);
            

            var productName = product.Localize("en", i => i.Name).Name;

            SetStatusMessage(string.Format("Product {0} successfully updated.", productName));
            
            return RedirectToAction("Index");
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