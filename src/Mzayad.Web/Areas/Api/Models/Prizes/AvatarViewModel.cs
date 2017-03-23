using Mzayad.Models;

namespace Mzayad.Web.Areas.Api.Models.Prizes
{
    public class AvatarViewModel
    {
        public int AvatarId { get; set; }
        public string Url { get; set; }
        public int Token { get; set; }

        public static AvatarViewModel Create(Avatar avatar)
        {
            return new AvatarViewModel
            {
                AvatarId = avatar.AvatarId,
                Url = avatar.Url,
                Token = avatar.Token ?? 0
            };
        }
    }
}