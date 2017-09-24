using Mazadaty.Models.Enum;
using Mazadaty.Models.Enums;
using System;

namespace Mazadaty.Web.Areas.Api.Models.Users
{
    public class UserProfileModel
    {
        public UserProfileStatus ProfileStatus { get; set; }

        public Gender? Gender { get; set; }

        public DateTime? Birthdate { get; set; }

        public int? AvatarId { get; set; }

        public string UserName { get; set; }

        public string ProfileUrl => $"https://www.zeedli.com/profiles/{UserName.ToLowerInvariant()}";
    }
}
