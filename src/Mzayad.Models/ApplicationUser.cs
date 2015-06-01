﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

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

        //public string CreatedLocalTime
        //{
        //    get { return DateTimeFormatter.ToLocalTime(CreatedUtc); }
        //}

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
