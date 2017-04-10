using Mzayad.Models;
using Mzayad.Models.Enum;
using Mzayad.Models.Enums;
using Mzayad.Services;
using Mzayad.Services.Payment;
using Mzayad.Web.Areas.Api.ErrorHandling;
using Mzayad.Web.Areas.Api.Models.Orders;
using Mzayad.Web.Core.Services;
using Mzayad.Web.Resources;
using OrangeJetpack.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace Mzayad.Web.Areas.Api.Controllers
{
    [RoutePrefix("api/orders")]
    [Authorize]
    public class OrdersController : ApplicationApiController
    {
        private readonly AuctionService _auctionService;
        private readonly OrderService _orderService;
        private readonly KnetService _knetService;
        private readonly ProductService _productService;

        public OrdersController(IAppServices appServices) : base(appServices)
        {
            _auctionService = new AuctionService(DataContextFactory, appServices.QueueService);
            _orderService = new OrderService(DataContextFactory);
            _knetService = new KnetService(DataContextFactory);
            _productService = new ProductService(DataContextFactory);
        }

        [Route("shipping")]
        public async Task<IHttpActionResult> GetShipping([FromUri]IList<int> productIds)
        {
            var products = await _productService.GetProductsByIds(productIds);
            var applayShippingCost = products.Any(i => !i.WaiveShippingCost);
            return Ok(applayShippingCost ? AppSettings.LocalShipping : 0);
        }

        [Route("create")]
        [HttpPost]
        public async Task<IHttpActionResult> CreateOrder(Models.Orders.OrderModel model)
        {
            if (!ModelState.IsValid)
            {
                return ModelStateError(ModelState);
            }

            if (model.Items == null || !model.Items.Any())
            {
                return CartError();
            }

            var validationResult = await ValidateOrderItems(model.AuctionId, model.Items.First());
            if (!validationResult)
            {
                return CartError();
            }

            var order = CreateOrderFromModel(model);
            await _orderService.CreateOrder(order);
            var result = await _knetService.InitTransaction(order, AuthService.CurrentUserId(), AuthService.UserHostAddress());
            var transaction = await _knetService.GetTransactionByOrderId(order.OrderId);
            return Ok(OrderCreateResult.Create(transaction, result));
        }

        [Route("{orderId:int}")]
        public async Task<IHttpActionResult> Get(int orderId)
        {
            var order = await _orderService.GetById(orderId);
            if (order.Items.Any())
            {
                order.Items = order.Items.Localize<OrderItem>(Language, LocalizationDepth.OneLevel).ToList();
            }
            var knetTransaction = await _knetService.GetTransactionByOrderId(orderId);
            return Ok(new { Order = OrderDetailModel.Create(order), KnetTransaction = KnetTransactionModel.Create(knetTransaction) });
        }

        private async Task<bool> ValidateOrderItems(int auctionId, OrderItem orderItem)
        {
            var auction = await _auctionService.GetAuctionById(auctionId);

            if (auction.ProductId != orderItem.ProductId)
            {
                return false;
            }

            if (!auction.BuyNowAvailable())
            {
                return false;
            }

            if (!auction.BuyNowPrice.HasValue)
            {
                return false;
            }

            orderItem.ItemPrice = auction.BuyNowPrice.Value;
            return true;
        }

        private static IHttpActionResult CartError()
        {
            var errorMessage = Global.OrderItemsValidationErrorMessage;

            return new ApiErrorResult(new ApiError
            {
                Type = ApiErrorType.ModelStateError,
                Message = errorMessage
            });
        }

        private Order CreateOrderFromModel(Models.Orders.OrderModel model)
        {
            var order = new Order
            {
                UserId = AuthService.CurrentUserId(),
                Address = model.ShippingAddress,
                Type = OrderType.BuyNow,
                PaymentMethod = PaymentMethod.Knet,
                Status = OrderStatus.PendingPayment,
                Shipping = AppSettings.LocalShipping,
                Items = model.Items.ToList(),
                SubmittedUtc = DateTime.UtcNow
            };

            order.RecalculateTotal();
            return order;
        }


    }
}

