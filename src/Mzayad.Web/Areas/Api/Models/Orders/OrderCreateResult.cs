using Mzayad.Models.Enum;
using Mzayad.Models.Payment;

namespace Mzayad.Web.Areas.Api.Models.Orders
{
    public class OrderCreateResult
    {
        public string RedirectUrl { get; set; }
        public string Message { get; set; }
        public PaymentStatus Status { get; set; }
        public int OrderId { get; set; }
        public string PaymentId { get; set; }


        public static OrderCreateResult Create(KnetTransaction transaction, InitTransactionResult result)
        {
            return new OrderCreateResult
            {
                RedirectUrl = result.RedirectUrl,
                Message = result.Message,
                Status = result.Status,
                OrderId = transaction.OrderId,
                PaymentId = transaction.PaymentId
            };
        }
    }
}