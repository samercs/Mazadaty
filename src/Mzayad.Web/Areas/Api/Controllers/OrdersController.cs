using Mzayad.Models;
using Mzayad.Models.Enum;
using Mzayad.Models.Enums;
using Mzayad.Services;
using Mzayad.Services.Payment;
using Mzayad.Web.Areas.Api.ErrorHandling;
using Mzayad.Web.Core.Services;
using Mzayad.Web.Resources;
using System;
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

        public OrdersController(IAppServices appServices) : base(appServices)
        {
            _auctionService = new AuctionService(DataContextFactory, appServices.QueueService);
            _orderService = new OrderService(DataContextFactory);
            _knetService = new KnetService(DataContextFactory);
        }

        [Route("shipping")]
        public IHttpActionResult GetShipping()
        {
            return Ok(AppSettings.LocalShipping);
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
            return Ok(result);
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

