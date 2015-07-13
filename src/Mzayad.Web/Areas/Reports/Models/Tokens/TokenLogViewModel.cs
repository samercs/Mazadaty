using System;
using Mzayad.Models;
using OrangeJetpack.Base.Core.Formatting;

namespace Mzayad.Web.Areas.Reports.Models.Tokens
{
    public class TokenLogViewModel
    {
        public int TokenLogId { get; set; }
        public DateTime Created { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        //public string UserFirstName { get; set; }
        //public string UserLastName { get; set; }
        public string UserFullName { get; set; }
        public string UserEmail { get; set; }
        public string ModifiedByUserId { get; set; }
        public string ModifiedByUserName { get; set; }
        //public string ModifiedByUserFirstName { get; set; }
        //public string ModifiedByUserLastName { get; set; }
        public string ModifiedByUserFullName { get; set; }
        public string ModifiedByUserEmail { get; set; }
        public int? OriginalTokenAmount { get; set; }
        public int ModifiedTokenAmount { get; set; }
        public int TokensAdded { get; set; }

        public static TokenLogViewModel Create(TokenLog subscriptionLog)
        {
            return new TokenLogViewModel
            {
                TokenLogId = subscriptionLog.TokenLogId,
                Created = subscriptionLog.CreatedUtc.ToLocalTime(),
                UserId = subscriptionLog.User.Id,
                UserName = subscriptionLog.User.UserName,
                //UserFirstName = subscriptionLog.User.FirstName,
                //UserLastName = subscriptionLog.User.LastName,
                UserFullName = NameFormatter.GetFullName(subscriptionLog.User.FirstName, subscriptionLog.User.LastName),
                UserEmail = subscriptionLog.User.Email,
                ModifiedByUserId = subscriptionLog.ModifiedByUser.Id,
                //ModifiedByUserFirstName = subscriptionLog.ModifiedByUser.FirstName,
                //ModifiedByUserLastName = subscriptionLog.ModifiedByUser.LastName,
                ModifiedByUserName = subscriptionLog.ModifiedByUser.UserName,
                ModifiedByUserFullName = NameFormatter.GetFullName(subscriptionLog.ModifiedByUser.FirstName, subscriptionLog.ModifiedByUser.LastName),
                ModifiedByUserEmail = subscriptionLog.ModifiedByUser.Email,
                OriginalTokenAmount = subscriptionLog.OriginalTokenAmount,
                ModifiedTokenAmount = subscriptionLog.ModifiedTokenAmount,
                TokensAdded = subscriptionLog.TokensAdded
            };
        }
    }
}
