using Mazadaty.Models.Enum;

namespace Mazadaty.Models.Payment
{
    public class InitTransactionResult
    {
        public PaymentStatus Status { get; set; }
        public string Message { get; set; }
        public string RedirectUrl { get; set; }
    }
}
