using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using Mzayad.Core.Formatting;
using Mzayad.Models.Enum;
using Mzayad.Services;
using Mzayad.Services.Payment;
using Mzayad.Web.Core.Services;

namespace Mzayad.Web.Controllers
{
    public class KnetController : ApplicationController
    {
        private readonly OrderService _orderService;
        private readonly KnetService _knetService;
        
        public KnetController(IAppServices appServices) : base(appServices)
        {
            _orderService = new OrderService(appServices.DataContextFactory);
            _knetService = new KnetService(appServices.DataContextFactory);
        }

        [HttpPost]
        public async Task<ActionResult> Transaction(string paymentId, string result, string auth, string @ref, string tranId, string postDate, string trackId)
        {
            var transaction = await _knetService.GetTransaction(paymentId);
            if (transaction == null)
            {
                throw new ArgumentException("Cannot find KNET transaction for payment ID: " + paymentId);
            }

            if (transaction.Order.Status == OrderStatus.Processing || transaction.Order.Status == OrderStatus.Shipped)
            {
                throw new Exception("Cannot POST transaction for order already processing or shipped. Order ID: " + transaction.OrderId + ", Request Params: " + RequestService.GetRequestParams());
            }

            var paymentStatus = result.Equals("CAPTURED", StringComparison.OrdinalIgnoreCase)
                ? PaymentStatus.Success
                : PaymentStatus.Failure;

            transaction.Status = paymentStatus;
            transaction.Result = result;
            transaction.AuthorizationNumber = auth;
            transaction.ReferenceNumber = @ref;
            transaction.TransactionId = tranId;
            transaction.PostDate = postDate;
            transaction.TrackId = trackId;
            transaction.RequestParams = RequestService.GetRequestParams();

            await _knetService.SaveTransaction(transaction);
            if (paymentStatus != PaymentStatus.Success)
            {
                return RedirectToAction("Error", new { paymentId });
            }

            await _orderService.SubmitOrder(transaction.Order, AuthService.UserHostAddress());

            var transactionNotes = string.Format("<p>&nbsp;</p>" +
                                                 "<table width='100%' cellpadding='8' cellspacing='0'>" +
                                                 "<tr><td colspan='2' style='background-color:#ebedee'><strong>KNET Transaction Details</strong></td>" +
                                                 "<tr><td width='1%' nowrap='nowrap' style='text-align:right;white-space:nowrap'><strong>Amount</strong></td><td>{7}</td>" +
                                                 "<tr><td width='1%' nowrap='nowrap' style='text-align:right;white-space:nowrap'><strong>Payment ID</strong></td><td>{0}</td>" +
                                                 "<tr><td width='1%' nowrap='nowrap' style='text-align:right;white-space:nowrap'><strong>Result Code</strong></td><td>{1}</td>" +
                                                 "<tr><td width='1%' nowrap='nowrap' style='text-align:right;white-space:nowrap'><strong>Transaction ID</strong></td><td>{2}</td>" +
                                                 "<tr><td width='1%' nowrap='nowrap' style='text-align:right;white-space:nowrap'><strong>Auth</strong></td><td>{3}</td>" +
                                                 "<tr><td width='1%' nowrap='nowrap' style='text-align:right;white-space:nowrap'><strong>Track ID</strong></td><td>{4}</td>" +
                                                 "<tr><td width='1%' nowrap='nowrap' style='text-align:right;white-space:nowrap'><strong>Ref No</strong></td><td>{5}</td>" +
                                                 "<tr><td width='1%' nowrap='nowrap' style='text-align:right;white-space:nowrap'><strong>Date/Time</strong></td><td>{6:dd MMM yyyy HH:mm:ss} GMT</td>" +
                                                 "</table>", transaction.PaymentId, transaction.Result,
                                                 transaction.TransactionId, transaction.AuthorizationNumber,
                                                 transaction.TrackId, transaction.ReferenceNumber,
                                                 DateTime.UtcNow, CurrencyFormatter.Format(transaction.Order.Total, Currency.Kwd));

            //await SendNotifications(transaction, transactionNotes);
            
            var redirectUrl = Url.Action("Success", "Order", new { transaction.OrderId, transaction.PaymentId }, RequestService.GetUrlScheme());

            return Content(string.Format("REDIRECT={0}", redirectUrl));
        }

        public async Task<ActionResult> Error(string paymentId)
        {
            var transaction = await _knetService.GetTransaction(paymentId);
            if (transaction == null)
            {
                return HttpNotFound();
            }

            return View(transaction);
        }
    }
}