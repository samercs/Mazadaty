namespace Mzayad.Models.Payment
{
    public class KnetResponse
    {
        public short TransactionCode { get; set; }
        public string PaymentId { get; set; }
        public string PaymentUrl { get; set; }
        public string ErrorMsg { get; set; }
        public string ResourcePath { get; set; }
    }
}