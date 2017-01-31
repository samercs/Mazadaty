using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Mindscape.Raygun4Net;
using Mzayad.Models;
using Mzayad.Models.Enum;
using Mzayad.Models.Enums;
using Mzayad.Services;
using Mzayad.Web.Areas.admin.Models.Auction;
using Mzayad.Web.Areas.admin.Models.Auctions;
using Mzayad.Web.Controllers;
using Mzayad.Web.Core.ActionResults;
using Mzayad.Web.Core.Attributes;
using Mzayad.Web.Core.Identity;
using Mzayad.Web.Core.Services;
using Mzayad.Web.Extensions;
using OrangeJetpack.Base.Core.Formatting;
using OrangeJetpack.Base.Web;
using OrangeJetpack.Localization;
using OrangeJetpack.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Mzayad.Web.Areas.admin.Controllers
{
    [RouteArea("admin"), RoutePrefix("auctions"), RoleAuthorize(Role.Administrator)]
    public class AuctionsController : ApplicationController
    {
        private readonly ProductService _productService;
        private readonly AuctionService _auctionService;
        private readonly NotificationService _notificationService;
        private readonly BidService _bidService;
        private readonly AutoBidService _autoBidService;

        public AuctionsController(IAppServices appServices) : base(appServices)
        {
            _productService = new ProductService(DataContextFactory);
            _auctionService = new AuctionService(DataContextFactory);
            _notificationService = new NotificationService(DataContextFactory);
            _bidService = new BidService(DataContextFactory);
            _autoBidService = new AutoBidService(DataContextFactory);
        }

        [Route("select-product")]
        public async Task<ActionResult> SelectProduct(string search = "")
        {
            var products = (await _productService.GetProductsWithoutCategory("en", search)).ToList();
            foreach (var product in products)
            {
                product.Description = StringFormatter.StripHtmlTags(product.Description);
            }

            var model = new Models.Products.IndexViewModel
            {
                Search = "",
                Products = products
            };

            return View(model);
        }

        public async Task<ActionResult> Create(int productId)
        {
            var product = await _productService.GetProduct(productId);
            if (product == null)
            {
                return HttpNotFound();
            }

            var model = await new AddEditViewModel().Hydrate(_productService, product);
            model.Auction.Title = product.Name;
            model.Auction.RetailPrice = product.RetailPrice;
            model.Auction.CreatedByUserId = AuthService.CurrentUserId();

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(int productId, AddEditViewModel model, LocalizedContent[] title, List<string> countryList)
        {
            ModelState["Auction.Title"].Errors.Clear();
            if (!ModelState.IsValid)
            {
                return View(await model.Hydrate(_productService, productId));
            }

            model.Auction.Title = title.Serialize();
            model.Auction.StartUtc = model.Auction.StartUtc.AddHours(-3);

            if (countryList != null && countryList.Count < model.GccCountryList.Count())
            {
                model.Auction.CountryList = string.Join(",", countryList);
            }
            else
            {
                model.Auction.CountryList = string.Empty;
            }

            var auction = await _auctionService.Add(model.Auction);

            if (auction.Status == AuctionStatus.Public)
            {
                await SendAuctionNotifications(auction);
            }

            SetStatusMessage("Auction has been added successfully.");

            return RedirectToAction("Index");
        }

        private async Task SendAuctionNotifications(Auction auction)
        {
            var notifications = await _notificationService.GetByCategories(auction.Product.Categories);

            var emailTemplate = await EmailTemplateService.GetByTemplateType(EmailTemplateType.AuctionCreated, "en");
            var urlHelper = new UrlHelper(ControllerContext.RequestContext);
            var auctionUrl = urlHelper.Action("Details", "Auctions", new { area = "", id = auction.AuctionId }, "https");
            var notificationsUrl = urlHelper.Action("Notifications", "User", new { area = "" }, "https");
            var productName = auction.Product.Localize("en", i => i.Name).Name;

            foreach (var user in notifications.Select(i => i.User).Distinct())
            {
                var email = new Email
                {
                    ToAddress = user.Email,
                    Subject = emailTemplate.Subject,
                    Message = emailTemplate.Message.Replace(new Dictionary<string, string>
                    {
                        {"{FirstName}", user.FirstName},
                        {"{AuctionUrl}", auctionUrl},
                        {"{ProductName}", productName},
                        {"{NotificationsUrl}", notificationsUrl}
                    })
                };

                try
                {
                    await MessageService.Send(email.WithTemplate());
                }
                catch (Exception ex)
                {
                    new RaygunClient().Send(ex);
                }
            }
        }

        public async Task<ActionResult> Index(string search = null)
        {
            var model = new IndexViewModel
            {
                Auctions = await GetAuctions(search),
                Search = search
            };

            return View(model);
        }

        private async Task<IEnumerable<Auction>> GetAuctions(string search = null)
        {
            var auctions = (await _auctionService.GetAllAuctions(search)).Localize("en", i => i.Title);

            foreach (var auction in auctions)
            {
                auction.Product.Localize("en", i => i.Name);
            }

            return auctions;
        }

        public async Task<JsonResult> GetAuctions([DataSourceRequest] DataSourceRequest request, string search = null)
        {
            var result = await GetAuctions(search);

            return Json(result.ToDataSourceResult(request));
        }

        public async Task<ExcelResult> DownloadExcel(string search = null)
        {
            var auctions = await GetAuctions(search);

            var results = auctions.Select(i => new
            {
                i.AuctionId,
                i.Title,
                i.Product.Name,
                StartUtc = i.StartUtc.ToString("yyyy/MM/dd HH:mm"),
                Status = i.Status.Description(),
                i.RetailPrice,
                i.ReservePrice,
                i.BidIncrement,
                i.Duration,
                i.MaximumBid,
                i.BuyNowEnabled,
                i.BuyNowPrice,
                i.BuyNowQuantity,
                CreatedUtc = i.CreatedUtc.ToString("yyyy/MM/dd HH:mm")
            });

            return Excel(results, "auctions");
        }

        [Route("edit/{id:int}")]
        public async Task<ActionResult> Edit(int id)
        {
            var auction = await _auctionService.GetAuction(id);
            if (auction == null)
            {
                SetStatusMessage("Sorry this auction not found", StatusMessageType.Warning);
                return RedirectToAction("Index", "Auctions");
            }

            var model = await new AddEditViewModel
            {
                Bids = await _bidService.GetBidHistoryForAuction(id),
                AutoBids = await _autoBidService.GetAutoBidsForAuction(id)
            }
            .Hydrate(_productService, auction);
            return View(model);
        }

        [Route("edit/{id:int}")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, AddEditViewModel model, LocalizedContent[] title, List<string> countryList)
        {
            var auction = await _auctionService.GetAuction(id);
            if (auction == null)
            {
                return HttpNotFound();
            }

            var isActivated = auction.Status != AuctionStatus.Public && model.Auction.Status == AuctionStatus.Public;

            auction.Title = title.Serialize();
            auction.BidIncrement = model.Auction.BidIncrement;
            auction.Duration = model.Auction.Duration;
            auction.MaximumBid = model.Auction.MaximumBid;
            auction.ProductId = model.Auction.ProductId;
            auction.RetailPrice = model.Auction.RetailPrice;
            auction.StartUtc = model.Auction.StartUtc;// AST >> UTS;
            auction.Status = model.Auction.Status;
            auction.BuyNowEnabled = model.Auction.BuyNowEnabled;
            auction.BuyNowPrice = model.Auction.BuyNowPrice;
            auction.BuyNowQuantity = model.Auction.BuyNowQuantity;
            if (countryList != null && countryList.Count < model.GccCountryList.Count())
            {
                auction.CountryList = string.Join(",", countryList);
            }
            else
            {
                auction.CountryList = string.Empty;
            }
            await _auctionService.Update(auction);

            if (isActivated)
            {
                await SendAuctionNotifications(auction);
            }

            var productName = auction.Product.Localize("en", i => i.Name).Name;

            SetStatusMessage($"Auction for <strong>{productName}</strong> has been updated successfully.");

            return RedirectToAction("Index", "Auctions");

        }

        [Route("delete/{auctionId:int}")]
        public async Task<ActionResult> Delete(int auctionId)
        {
            var auction = await _auctionService.GetAuction(auctionId);
            if (auction == null)
            {
                SetStatusMessage("Sorry this auction not found", StatusMessageType.Warning);
                return RedirectToAction("Index", "Auctions");
            }

            return DeleteConfirmation("Delete Auction", "Are you sure you want to permanently delete this cuisine?");
        }

        [Route("delete/{auctionId:int}")]
        [HttpPost]
        public async Task<ActionResult> DeleteAuction(int auctionId)
        {
            await _auctionService.DeleteAuction(auctionId);
            SetStatusMessage("Auction was successfully deleted");
            return RedirectToAction("Index", "Auctions");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Activate(int auctionId)
        {
            var auction = await _auctionService.GetAuction(auctionId);
            if (auction == null)
            {
                return HttpNotFound();
            }

            if (auction.Status != AuctionStatus.Public)
            {
                auction.Status = AuctionStatus.Public;

                await _auctionService.Update(auction);

                await SendAuctionNotifications(auction);
            }

            var productName = auction.Product.Localize("en", i => i.Name).Name;

            SetStatusMessage(string.Format("Auction for <strong>{0}</strong> successfully activated and now available to the public.", productName));

            return RedirectToAction("Index");
        }

        [Route("{auctionId:int}/auto-bids")]
        public async Task<ActionResult> AutoBids(int auctionId)
        {
            var autoBids = await _autoBidService.GetAutoBidsForAuction(auctionId);
            return View(autoBids);
        }

        public async Task<JsonResult> GetAutoBids([DataSourceRequest] DataSourceRequest request, int auctionId)
        {
            var result = await _autoBidService.GetAutoBidsForAuction(auctionId);

            return Json(result.ToDataSourceResult(request));
        }


    }
}