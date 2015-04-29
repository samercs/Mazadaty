using Mzayad.Models;
using Mzayad.Web.Models.Shared;

namespace Mzayad.Web.Models.Order
{
    public class ShippingAddressViewModel
    {
        public Mzayad.Models.Order Order { get; set; }
        public AddressViewModel AddressViewModel { get; set; }
        public ShippingAddress ShippingAddress { get; set; }

        public string PhoneCountryCode { get; set; }
        public string PhoneLocalNumber { get; set; }
    }
}
