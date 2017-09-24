using Mazadaty.Models.Enums;
using Mazadaty.Web.Models.Shared;
using Mazadaty.Web.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace Mazadaty.Web.Models.Account
{
    public class UserAccountViewModel
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [StringLength(256)]
        [EmailAddress]
        public string Email { get; set; }

        [Required, StringLength(4, MinimumLength = 1), RegularExpression(@"^\+?([0-9]{1,3})$")]
        public string PhoneCountryCode { get; set; }

        [Required, StringLength(15, MinimumLength = 7), RegularExpression(@"^[0-9\s-\(\)\.]{7,15}$")]
        public string PhoneNumber { get; set; }

        public AddressViewModel Address { get; set; }

        public Gender? Gender { get; set; }

        public DateTime? Birthdate { get; set; }

        public PhoneNumberViewModel PhoneNumberViewModel { get; set; }

        public IList<SelectListItem> GenderList
        {
            get
            {
                return Enum.GetValues(typeof(Gender)).Cast<Gender>().Select(i => new SelectListItem
                {
                    Text = Global.ResourceManager.GetString(i.ToString()),
                    Value = ((int)i).ToString(),
                    Selected = i == Gender
                }).ToList();
            }
        }
    }
}
