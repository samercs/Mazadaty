using Mazadaty.Models.Payment;

namespace Mazadaty.Web.Models.Order
{
    public class DetailsViewModel
    {
        public OrderViewModel Order { get; set; }
        public KnetTransaction KnetTransaction { get; set; }
        public string RedirectUrl { get; set; }
    }
}
