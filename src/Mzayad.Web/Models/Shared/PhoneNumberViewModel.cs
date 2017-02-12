using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Mzayad.Web.Models.Shared
{
    public class PhoneNumberViewModel
    {
        public string CountryCode { get; set; }

        [Required]
        public string PhoneCountryCode { get; set; }

        [Required, StringLength(15, MinimumLength = 7)]
        public string PhoneLocalNumber { get; set; }

        public IEnumerable<SelectListItem> CountriesList { get; set; }

    }
}