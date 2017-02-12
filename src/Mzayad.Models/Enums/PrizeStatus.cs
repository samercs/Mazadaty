using System.ComponentModel;

namespace Mzayad.Models.Enums
{
    public enum PrizeStatus
    {
        [Description("This prize is hidden")]
        Hidden = 1,
        [Description("This prize will display in site")]
        Public = 2
    }
}