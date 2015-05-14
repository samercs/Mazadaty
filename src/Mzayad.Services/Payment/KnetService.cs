using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Mzayad.Data;
using Mzayad.Models;
using Mzayad.Models.Enum;
using Mzayad.Models.Payment;
using Newtonsoft.Json;

namespace Mzayad.Services.Payment
{
    public class KnetService : ServiceBase
    {
        private readonly OrderService _orderService;
        
        public KnetService(IDataContextFactory dataContextFactory) : base(dataContextFactory)
        {
            _orderService = new OrderService(dataContextFactory);
        }

        public async Task<KnetTransaction> GetTransaction(string paymentId)
        {
            using (var dc = DataContext())
            {
                return await dc.KnetTransactions
                    .Include(i => i.Order)
                    .Include(i => i.Order.Address)
                    .Include(i => i.Order.Items)
                    .SingleOrDefaultAsync(i => i.PaymentId == paymentId);
            }
        }

        public async Task<IEnumerable<KnetTransaction>> GetTransactions()
        {
            using (var dc = DataContext())
            {
                return await dc.KnetTransactions
                    .Include(i => i.Order)
                    .OrderByDescending(i => i.CreatedUtc)
                    .ToListAsync();
            }
        }

        public async Task<IEnumerable<KnetTransaction>> GetTransactions(Order order)
        {
            using (var dc = DataContext())
            {
                return await dc.KnetTransactions
                    .Include(i => i.Order)
                    .Where(i => i.OrderId == order.OrderId)
                    .OrderBy(i => i.CreatedUtc)
                    .ToListAsync();
            }
        }

        private async Task<InitTransactionResult> GetFakeTransaction(Order order, string userId, string userHostAddress)
        {
            var paymentId = DateTime.UtcNow.Ticks.ToString();

            await CreateTransaction(
                    order, paymentId,
                    userId,
                    userHostAddress);

            return new InitTransactionResult
            {
                RedirectUrl = string.Format("/knet/test?PaymentId={0}", paymentId)
            };
        }

        public async Task<InitTransactionResult> InitTransaction(Order order, string userId, string userHostAddress)
        {
            return await GetFakeTransaction(order, userId, userHostAddress);
                  
            var result = new InitTransactionResult();

            using (var client = new HttpClient())
            {
                const string url = "http://knetprocessor.cloudapp.net/api/knet";

                var responseMessage = await client.PostAsJsonAsync(url, new
                {
                    Amount = order.Total
                });

                var responseContent = await responseMessage.Content.ReadAsStringAsync();

                var knetResponse = JsonConvert.DeserializeObject<KnetResponse>(responseContent);
                if (knetResponse.TransactionCode != 0)
                {
                    result.Status = PaymentStatus.Failure;
                    result.Message = knetResponse.ErrorMsg;
                    return result;
                }

                await CreateTransaction(
                    order, knetResponse.PaymentId,
                    userId,
                    userHostAddress);

                result.Status = PaymentStatus.Success;
                result.RedirectUrl = knetResponse.PaymentUrl;
                return result;
            }
        }

        private async Task<KnetTransaction> CreateTransaction(Order order, string paymentId, string userId, string userHostAddress)
        {
            await _orderService.UpdateStatus(order, OrderStatus.PendingPayment, userHostAddress, userId);

            using (var dc = DataContext())
            {
                var knetTransaction = new KnetTransaction
                {
                    OrderId = order.OrderId,
                    Status = PaymentStatus.Pending,
                    PaymentId = paymentId
                };

                dc.KnetTransactions.Add(knetTransaction);

                await dc.SaveChangesAsync();

                return knetTransaction;
            }
        }

        public async Task<KnetTransaction> SaveTransaction(KnetTransaction transaction)
        {
            using (var dc = DataContext())
            {
                dc.SetModified(transaction);
                await dc.SaveChangesAsync();
                return transaction;
            }
        }
    }
}
