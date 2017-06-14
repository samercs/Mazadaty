namespace Mzayad.Web.Models.Order
{
    public class OrderViewModel : Mzayad.Models.Order
    {
        public static OrderViewModel Create(Mzayad.Models.Order order)
        {        
            return new OrderViewModel
            {
                OrderId = order.OrderId,
                UserId = order.UserId,
                AllowPhoneSms = order.AllowPhoneSms,
                Status = order.Status,
                Subtotal = order.Subtotal,
                Shipping = order.Shipping,
                Total = order.Total,
                PaymentMethod = order.PaymentMethod,
                SubmittedUtc = order.SubmittedUtc,
                ShippedUtc = order.ShippedUtc,
                User = order.User,
                AddressId = order.AddressId,
                Address = order.Address,
                Items = order.Items,
                Logs = order.Logs
            };
        }
    }
}
