using Mzayad.Models;
using Mzayad.Services.Identity;
using Mzayad.Web.Core.Identity;
using OrangeJetpack.Base.Core.Formatting;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Mzayad.Web.Areas.admin.Models.Users
{
    public class DetailsViewModel
    {
        public string UserId { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        [StringLength(256)]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        [DataType(DataType.EmailAddress)]
        [StringLength(256)]
        public string Email { get; set; }

        public DateTime CreatedUtc { get; set; }
        public DateTime? SubscriptionUtc { get; set; }
        public string AvatarUrl { get; set; }
        public List<SelectListItem> Roles { get; set; }

        public bool IsLocked { get; set; }

        public IReadOnlyCollection<SubscriptionLog> SubscriptionLogs { get; set; }
        public IReadOnlyCollection<TokenLog> TokenLogs { get; set; }
        public IReadOnlyCollection<UserTrophy> Trophies { get; set; }
        public IReadOnlyCollection<Mzayad.Models.Auction> Auctions { get; set; }
        public IReadOnlyCollection<Bid> Bids { get; set; }

        public async Task<DetailsViewModel> Hydrate(ApplicationUser user, UserService userService)
        {
            UserId = user.Id;
            FirstName = user.FirstName;
            LastName = user.LastName;
            UserName = user.UserName;
            Email = user.Email;
            CreatedUtc = user.CreatedUtc;
            SubscriptionUtc = user.SubscriptionUtc;
            AvatarUrl = user.AvatarUrl;

            var roles = (await userService.GetRolesForUser(user.Id));

            Roles = (from Role role in Enum.GetValues(typeof(Role))
                     select new SelectListItem
                     {
                         Text = role.ToString(),
                         Value = EnumFormatter.Description(role),
                         Selected = roles.Contains(role.ToString())
                     }).ToList();

            
            return this;
        }
    }
}