using System;
using System.ComponentModel.DataAnnotations;

namespace Mzayad.Web.Models.Account
{
    public class NeedPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
    
    public class SignInViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

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
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(int.MaxValue, MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
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
