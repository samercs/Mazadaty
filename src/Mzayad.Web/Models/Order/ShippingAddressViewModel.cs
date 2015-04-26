using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mzayad.Models;
using Mzayad.Web.Models.Shared;

namespace Mzayad.Web.Models.Order
{
    public class ShippingAddressViewModel
    {
        public Mzayad.Models.Order Order { get; set; }
        public AddressViewModel AddressViewModel { get; set; }
        public ShippingAddress ShippingAddress { get; set; }

        public string PhoneNumberCountryCode { get; set; }
        public string PhoneNumberNumber { get; set; }

    }
}
