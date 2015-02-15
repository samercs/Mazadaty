using System.Collections.Generic;
using System.Web.Mvc;
using Mzayad.Models;

namespace Mzayad.Web.Models.Shared
{
    public class AddressViewModel : Address
    {
        public List<SelectListItem> CountriesList { get; set; }

        public AddressViewModel()
        {
            
        }

        public AddressViewModel(Address address)
        {
            AddressId = address.AddressId;
            AddressLine1 = address.AddressLine1;
            AddressLine2 = address.AddressLine2;
            AddressLine3 = address.AddressLine3;
            AddressLine4 = address.AddressLine4;
            CityArea = address.CityArea;
            StateProvince = address.StateProvince;
            PostalCode = address.PostalCode;
            CountryCode = address.CountryCode;
        }

        public AddressViewModel Hydrate()
        {
            CountriesList = new List<SelectListItem>();
            CountriesList.Add(new SelectListItem()
            {
                Text = "Kuwait",
                Value = "KW",
                Selected = true
            });

            return this;
        }

        //public AddressViewModel Hydrate(IShipmentSettings shipmentSettings)
        //{
        //    CountriesList = shipmentSettings.GetCountries()
        //        .Select(i => new SelectListItem
        //        {
        //            Text = i.Name,
        //            Value = i.IsoCode,
        //            Selected = i.IsoCode.Equals(CountryCode)
        //        })
        //        .ToList();
            
        //    return this;
        //}
    }
}