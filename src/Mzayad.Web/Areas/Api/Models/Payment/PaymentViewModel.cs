using Humanizer;
using Mzayad.Models.Payment;

namespace Mzayad.Web.Areas.Api.Models.Payment
{
    public class PaymentViewModel
    {
        public string PaymentId { get; set; }
        public int TransactionId { get; set; }
        public int Status { get; set; }
        public string StatusDescription { get; set; }

        public static PaymentViewModel Create(KnetTransaction paymentTransaction)
        {
            return new PaymentViewModel
            {
                PaymentId = paymentTransaction.PaymentId,
                TransactionId = paymentTransaction.PaymentTransactionId,
                Status = (int)paymentTransaction.Status,
                StatusDescription = paymentTransaction.Status.Humanize()
            };
        }
    }
}