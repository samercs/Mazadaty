using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mzayad.Models.Enum
{
    public enum AuctionStatus
    {
        [Description("Auction is not visible to users")]
        Hidden = 1,
        [Description("Auction is visible to users")]
        Public =2
    }
}
