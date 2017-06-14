using Mzayad.Models;
using Mzayad.Web.Models.Shared;

namespace Mzayad.Web.Models.Order
{
    public class ShippingAddressViewModel
    {
        public Mzayad.Models.Order Order { get; set; }
        public AddressViewModel AddressViewModel { get; set; }
        public ShippingAddress ShippingAddress { get; set; }
        public PhoneNumberViewModel PhoneNumberViewModel { get; set; }

        public ShippingAddressViewModel Hydrate()
        {
            PhoneNumberViewModel = new PhoneNumberViewModel
            {
                CountriesList = AddressViewModel.CountriesList,
                CountryCode = AddressViewModel.CountryCode,
                PhoneCountryCode = ShippingAddress.PhoneCountryCode,
                PhoneLocalNumber = ShippingAddress.PhoneLocalNumber
            };

            return this;
        }
    }
}
