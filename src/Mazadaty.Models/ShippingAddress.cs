using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using OrangeJetpack.Base.Core.Formatting;

namespace Mazadaty.Models
{
    [Table("ShipmentAddress")]
    public class ShippingAddress : Address
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string PhoneCountryCode { get; set; }

        [Required]
        public string PhoneLocalNumber { get; set; }

        public static ShippingAddress Create(ApplicationUser user)
        {
            return new ShippingAddress
            {
                AddressLine1 = user.Address?.AddressLine1 ?? "",
                AddressLine2 = user.Address?.AddressLine2 ?? "",
                AddressLine3 = user.Address?.AddressLine3 ?? "",
                AddressLine4 = user.Address?.AddressLine4 ?? "",
                CityArea = user.Address?.CityArea ?? "",
                CountryCode = user.Address?.CountryCode ?? "",
                PostalCode = user.Address?.PostalCode ?? "",
                StateProvince = user.Address?.StateProvince ?? "",

                Name = NameFormatter.GetFullName(user.FirstName, user.LastName),
                PhoneCountryCode = user.PhoneCountryCode,
                PhoneLocalNumber = user.PhoneNumber
            };
        }
    }
}
