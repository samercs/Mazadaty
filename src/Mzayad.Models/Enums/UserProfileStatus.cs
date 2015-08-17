using System.ComponentModel;

namespace Mzayad.Models.Enum
{
    public enum UserProfileStatus
    {
        [Description("Your profile is <strong>visible</strong> to other signed-in users.")]
        Public =1,
        [Description("Your profile and details are <strong>hidden</strong> from everyone.")]
        Private =2
    }
}
