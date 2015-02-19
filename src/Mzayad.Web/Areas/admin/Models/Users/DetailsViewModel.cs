using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Mzayad.Models;
using Mzayad.Web.Core.Identity;
using Mzayad.Web.Core.Services;

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
        [EmailAddress]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        public DateTime CreatedUtc { get; set; }

        public List<SelectListItem> Roles { get; set; }

        public async Task<DetailsViewModel> Hydrate(ApplicationUser user, IAuthService authService)
        {
            UserId = user.Id;
            FirstName = user.FirstName;
            LastName = user.LastName;
            Email = user.Email;
            CreatedUtc = user.CreatedUtc;
            
            var roles = (await authService.GetRolesForUser(user.Id));

            Roles = (from Role role in Enum.GetValues(typeof(Role))
                     orderby (int)role
                     select new SelectListItem
                     {
                         Text = role.ToString(), 
                         Value = role.ToString(), 
                         Selected = roles.Contains(role.ToString())
                     }).ToList();
            
            return this;
        }
    }
}