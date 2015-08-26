﻿using Mzayad.Models;
using Mzayad.Models.Enum;
using Mzayad.Services;
using Mzayad.Web.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Mzayad.Web.Models.User
{
    public class EditProfileModel
    {
        public ApplicationUser User { get; set; }
        public IReadOnlyCollection<SelectListItem> PrivacyList { get; set; }
        public IReadOnlyCollection<Avatar> Avatars { get; set; }

        public async Task<EditProfileModel> Hydrate(AvatarService avatarService, ApplicationUser user)
        {
            User = user;
            PrivacyList = Enum.GetValues(typeof(UserProfileStatus))
                        .Cast<UserProfileStatus>()
                        .Select(i => new SelectListItem()
                        {
                            Text = i.ToString() + " - " + i.Description(),
                            Value = i.ToString(),
                            Selected = i == User.ProfileStatus
                        })
                        .ToList();

            Avatars = await avatarService.GetAll();

            return this;
        }
    }
}
