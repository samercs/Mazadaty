using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Mzayad.Models;
using Mzayad.Models.Enum;
using Mzayad.Services;
using Mzayad.Web.Extensions;

namespace Mzayad.Web.Models.User
{
    public class EditProfileModel
    {
        public UserProfile UserProfile { get; set; }

        public IEnumerable<SelectListItem> PrivacyList { get; set; }

        public IEnumerable<Avatar> Avatars { get; set; }

        public async Task<EditProfileModel> Hydrate(AvatarService avatarService,UserProfile userProfile)
        {
            UserProfile = userProfile;
            PrivacyList = Enum.GetValues(typeof(UserProfileStatus))
                        .Cast<UserProfileStatus>()
                        .Select(i => new SelectListItem()
                        {
                            Text = i.ToString() + " - " + i.Description(),
                            Value = i.ToString(),
                            Selected = i == UserProfile.Status
                        }
                        );
            Avatars = await avatarService.GetAll();

            return this;
        }
    }
}
