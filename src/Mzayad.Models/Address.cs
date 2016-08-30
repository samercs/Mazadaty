using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using OrangeJetpack.Base.Data;

namespace Mzayad.Models
{
    public class Address : EntityBase
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AddressId { get; set; }
        
        [Required]
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string AddressLine4 { get; set; }
        
        [Required]
        public string CityArea { get; set; }
        public string StateProvince { get; set; }
        public string PostalCode { get; set; }

        [Required]
        public string CountryCode { get; set; }
    }
}