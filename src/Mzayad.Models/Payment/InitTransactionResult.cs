using Mzayad.Models.Enum;

namespace Mzayad.Models.Payment
{
    public class InitTransactionResult
    {
        public PaymentStatus Status { get; set; }
        public string Message { get; set; }
        public string RedirectUrl { get; set; }
    }
}