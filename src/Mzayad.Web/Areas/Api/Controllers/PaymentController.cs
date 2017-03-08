using Mzayad.Services;
using Mzayad.Services.Payment;
using Mzayad.Web.Areas.Api.Models.Payment;
using Mzayad.Web.Core.Services;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace Mzayad.Web.Areas.Api.Controllers
{
    [RoutePrefix("api/payment")]
    public class PaymentController : ApplicationApiController
    {
        private readonly KnetService _knetService;
        private readonly OrderService _orderService;
        public PaymentController(IAppServices appServices) : base(appServices)
        {
            _knetService = new KnetService(DataContextFactory);
            _orderService = new OrderService(DataContextFactory);
        }
        [Route("{paymentId}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetStatus(string paymentId)
        {
            var payment = await _knetService.GetTransaction(paymentId);
            if (payment == null)
            {
                return NotFound();
            }
            return Ok(PaymentViewModel.Create(payment));
        }

        [Route("~/api/order/{orderId:int}/payment")]
        [HttpGet]
        public async Task<IHttpActionResult> GetOrderStatus(int orderId)
        {
            var order = await _orderService.GetById(orderId);
            if (order == null)
            {
                return NotFound();
            }
            var payments = await _knetService.GetTransactions(order);
            if (payments == null || !payments.Any())
            {
                return NotFound();
            }
            return Ok(payments.Select(PaymentViewModel.Create));

        }
    }
}
