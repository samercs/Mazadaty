using Mazadaty.Models;
using Mazadaty.Models.Enum;
using Mazadaty.Models.Enums;
using Mazadaty.Services;
using Mazadaty.Web.Extensions;
using Mazadaty.Web.Resources;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebGrease.Css.Extensions;

namespace Mazadaty.Web.Models.User
{
    public class EditProfileModel
    {
        public ApplicationUser User { get; set; }
        public IReadOnlyCollection<SelectListItem> PrivacyList { get; set; }
        public IReadOnlyCollection<Avatar> Avatars { get; set; }
        public IEnumerable<int> UserAvatarIds { get; set; }
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
            var allAvatar = await avatarService.GetAll();
            var userAvatar = await avatarService.GetUserAvatars(user);
            userAvatar = userAvatar.OrderByDescending(i => i.IsPremium);
            var userAvatarIds = userAvatar.Select(i => i.AvatarId);
            var unOwenedUserAvatar = allAvatar.Where(i => !userAvatarIds.Contains(i.AvatarId));
            unOwenedUserAvatar = unOwenedUserAvatar.OrderBy(i => i.IsPremium);

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

            Avatars = userAvatar.Concat(unOwenedUserAvatar).ToSafeReadOnlyCollection();
            UserAvatarIds = userAvatarIds;
            return this;
        }
    }
}
