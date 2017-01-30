using Mzayad.Models.Enum;
using Mzayad.Models.Enums;
using System;

namespace Mzayad.Web.Areas.Api.Models.Users
{
    public class UserProfileModel
    {
        public UserProfileStatus ProfileStatus { get; set; }

        public Gender Gender { get; set; }

        public DateTime? Birthdate { get; set; }

        public int? AvatarId { get; set; }
    }
}