using System;
using System.ComponentModel.DataAnnotations;
using Mzayad.Web.Models.Shared;

namespace Mzayad.Web.Models.Account
{
    public class NeedPasswordViewModel
    {
        [Required]
        [StringLength(256)]
        [EmailAddress]
        public string Email { get; set; }
    }
    
    public class SignInViewModel
    {
        [Required]
        [StringLength(256)]
        public string UsernameOrEmail { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
        public string ReturnUrl { get; set; }
    }

    public class RegisterViewModel
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }
        
        [Required]
        [StringLength(256)]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(256)]
        [RegularExpression(@"^[a-zA-Z0-9-\._]+$")]
        public string UserName { get; set; }

        [Required]
        [StringLength(int.MaxValue, MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required, StringLength(4, MinimumLength = 1), RegularExpression(@"^\+?([0-9]{1,3})$")]
        public string PhoneCountryCode { get; set; }

        [Required, StringLength(15, MinimumLength = 7), RegularExpression(@"^[0-9\s-\(\)\.]{7,15}$")]
        public string PhoneNumber { get; set; }

        public AddressViewModel Address { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required]
        [StringLength(Int32.MaxValue, MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
