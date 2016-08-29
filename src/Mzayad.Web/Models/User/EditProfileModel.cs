using Mzayad.Models;
using Mzayad.Models.Enum;
using Mzayad.Services;
using Mzayad.Web.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Mzayad.Models.Enums;
using Mzayad.Web.Resources;

namespace Mzayad.Web.Models.User
{
    public class EditProfileModel
    {
        public ApplicationUser User { get; set; }
        public IReadOnlyCollection<SelectListItem> PrivacyList { get; set; }
        public IReadOnlyCollection<Avatar> Avatars { get; set; }
        public int? BirthDay { get; set; }
        public int? BirthMonth { get; set; }
        public int? BirthYear { get; set; }

        public IEnumerable<SelectListItem> GenderList
        {
            get
            {
                return Enum.GetValues(typeof(Gender)).Cast<Gender>().Select(i => new SelectListItem
                {
                    Text = Global.ResourceManager.GetString(i.ToString()),
                    Value = ((int)i).ToString(),
                    Selected = User.Gender == i
                }).ToList();
            }
        }

        public List<SelectListItem> DayList
        {
            get
            {
                return Enumerable.Range(1, 31).Select(i => new SelectListItem
                {
                    Text = i.ToString(),
                    Value = i.ToString(),
                    Selected = User.Birthdate.HasValue && User.Birthdate.Value.Day == i
                }).ToList();
            }
        }

        public List<SelectListItem> MonthList
        {
            get
            {
                return Enumerable.Range(1, 12).Select(i => new SelectListItem
                {
                    Text = CultureInfo.InvariantCulture.DateTimeFormat.GetMonthName(i),
                    Value = i.ToString(),
                    Selected = User.Birthdate.HasValue && User.Birthdate.Value.Month == i
                }).ToList();
            }
        }

        public List<SelectListItem> YearList
        {
            get
            {
                return Enumerable.Range(1940, 70).Select(i => new SelectListItem
                {
                    Text = i.ToString(),
                    Value = i.ToString(),
                    Selected = User.Birthdate.HasValue && User.Birthdate.Value.Year == i
                }).ToList();
            }
        }

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
