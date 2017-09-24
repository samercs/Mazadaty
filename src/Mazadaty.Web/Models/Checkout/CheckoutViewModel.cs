using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Mazadaty.Web.Models.Checkout
{
    public class CheckoutViewModel : IValidatableObject
    {
        [Required]
        [EmailAddress]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }
        public bool RememberMe { get; set; }

        public CheckoutMode CheckoutMode { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (CheckoutMode != CheckoutMode.AsGuest && string.IsNullOrEmpty(Password))
            {
                yield return new ValidationResult("Password is require", new[] { "Password" });
            }
        }
    }
}
