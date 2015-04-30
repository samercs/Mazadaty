using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mzayad.Models
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
    }
}
