using Mazadaty.Models;
using Mazadaty.Models.Enum;
using Mazadaty.Models.Enums;
using Mazadaty.Services;
using Mazadaty.Services.Payment;
using Mazadaty.Web.Areas.Api.ErrorHandling;
using Mazadaty.Web.Areas.Api.Models.Orders;
using Mazadaty.Web.Core.Services;
using OrangeJetpack.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace Mazadaty.Web.Areas.Api.Controllers
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
            var user = await AuthService.CurrentUser();
            var products = await _productService.GetProductsByIds(productIds);
            var shipping = OrderService.CalculateShipping(products, user);

            return Ok(shipping);
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
                return CartError("Order item is null or empty");
            }

            var validationResult = await ValidateOrderItems(model.AuctionId, model.Items, model.Type);
            if (!string.IsNullOrEmpty(validationResult))
            {
                return CartError(validationResult);
            }


            Order order = null;
            if (model.Type == OrderType.Auction && model.OrderId != 0)
            {
                order = await _orderService.GetById(model.OrderId);
            }
            else
            {
                order = CreateOrderFromModel(model);
                await _orderService.CreateOrder(order);
            }
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

        private async Task<string> ValidateOrderItems(int auctionId, IReadOnlyCollection<OrderItem> orderItems, OrderType type)
        {
            var auction = await _auctionService.GetAuctionById(auctionId);

            if (auctionId != -1)
            {
                var orderItem = orderItems.First();
                if (type == OrderType.BuyNow)
                {
                    return ValidateBuyNowForAuction(auction, orderItem);
                }
                else if (type == OrderType.Auction)
                {
                    if (string.IsNullOrEmpty(auction.WonByUserId))
                    {
                        return "Auction does not have WonByUserId";
                    }
                    if (!auction.WonAmount.HasValue)
                    {
                        return "Auction does not have WonAmount";
                    }
                    if (auction.WonAmount.Value != orderItem.ItemPrice)
                    {
                        return "Order item price error";
                    }
                }
            }
            else
            {
                //Shopping cart order.
                var itemAuctionIds = orderItems.Select(i => i.AuctionId).ToList();
                var itemsAuctions = await _auctionService.GetByIds(itemAuctionIds);
                foreach (var orderItem in orderItems)
                {
                    if (!orderItem.AuctionId.HasValue)
                    {
                        return "Order item does not have AuctionId";
                    }
                    var itemAuction = itemsAuctions.FirstOrDefault(i => i.AuctionId == orderItem.AuctionId.Value);
                    var validateResult = ValidateBuyNowForAuction(itemAuction, orderItem);
                    if (!string.IsNullOrEmpty(validateResult))
                    {
                        return validateResult;
                    }
                }
            }
            return String.Empty;
        }

        private string ValidateBuyNowForAuction(Auction auction, OrderItem orderItem)
        {
            if (auction == null)
            {
                return "Auction not found.";
            }
            if (orderItem.Quantity != 1)
            {
                return "Order item quantity should be 1";
            }
            if (!auction.BuyNowPrice.HasValue)
            {
                return "Auction buy now prices not available";
            }
            if (!auction.BuyNowQuantity.HasValue)
            {
                return "Action buy now quantity not available";
            }
            if (!auction.BuyNowAvailable())
            {
                return "Auction does not available for buy now";
            }
            if (auction.BuyNowPrice.Value != orderItem.ItemPrice)
            {
                return "Order item prices does not equals to auction buy now";
            }
            if (auction.BuyNowQuantity.Value <= 0)
            {
                return "Action buy now quantity is less than 1";
            }
            return string.Empty;
        }

        private static IHttpActionResult CartError(string message)
        {
            //var errorMessage = Global.OrderItemsValidationErrorMessage;
            return new ApiErrorResult(new ApiError
            {
                Type = ApiErrorType.ModelStateError,
                Message = message
            });
        }

        private Order CreateOrderFromModel(Models.Orders.OrderModel model)
        {
            var order = new Order
            {
                UserId = AuthService.CurrentUserId(),
                Address = model.ShippingAddress,
                Type = model.Type,
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

