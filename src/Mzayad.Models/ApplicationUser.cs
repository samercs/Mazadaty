using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Mzayad.Models.Enum;
using Mzayad.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Mzayad.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
      
        [Required, DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedUtc { get; set; }

        [Required, StringLength(4, MinimumLength = 1)]
        public string PhoneCountryCode { get; set; }

        [Required, StringLength(15, MinimumLength = 7)]
        public override string PhoneNumber { get; set; }

        public int? AddressId { get; set; }

        public DateTime? SubscriptionUtc { get; set; }

        public virtual Address Address { get; set; }
        
        public virtual IEnumerable<CategoryNotification> Notifications { get; set; }
        public virtual IEnumerable<WishList> WishLists { get; set; }
        public virtual ICollection<SessionLog> SessionLogs { get; set; }

        [DataType(DataType.Url)]
        public string AvatarUrl { get; set; }
        public int Level { get; set; }
        public int Xp { get; set; }
        public int Tokens { get; set; }
        public Gender? Gender { get; set; }
        public DateTime? Birthdate { get; set; }

        [Required]
        public UserProfileStatus ProfileStatus { get; set; }

        public string ProfileUrl => $"https://www.zeedli.com/profiles/{UserName.ToLowerInvariant()}";

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            return await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager, string authenticationType)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            // Add custom user claims here
            return userIdentity;
        }
    }
}
