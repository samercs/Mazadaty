using Mzayad.Models.Payment;

namespace Mzayad.Web.Models.Order
{
    public class DetailsViewModel
    {
        public OrderViewModel Order { get; set; }
        public KnetTransaction KnetTransaction { get; set; }
        public string RedirectUrl { get; set; }
    }
}