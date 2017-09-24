using System;
using Mazadaty.Models.Payment;

namespace Mazadaty.Web.Areas.Api.Models.Orders
{
    public class KnetTransactionModel
    {
        public string PaymentId { get; set; }
        public string ResultCode { get; set; }
        public string TransactionId { get; set; }
        public string AuthCode { get; set; }
        public string TrackId { get; set; }
        public string RefNo { get; set; }
        public DateTime Date { get; set; }

        public static KnetTransactionModel Create(KnetTransaction transaction)
        {
            return new KnetTransactionModel
            {
                PaymentId = transaction.PaymentId,
                ResultCode = transaction.Result,
                TransactionId = transaction.TransactionId,
                AuthCode = transaction.AuthorizationNumber,
                TrackId = transaction.TrackId,
                Date = transaction.CreatedUtc,
                RefNo = transaction.ReferenceNumber
            };
        }
    }
}
