using System.ComponentModel;

namespace Mzayad.Models.Enums
{
    public enum AuctionStatus
    {
        [Description("Auction is not visible to users")]
        Hidden = 1,
        
        [Description("Auction is visible to users")]
        Public,

        [Description("Auction is visible but closed")]
        Closed
    }
}
