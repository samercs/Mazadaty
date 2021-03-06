using Mazadaty.Data;
using Mazadaty.Models;
using Mazadaty.Services.Identity;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Mazadaty.Services
{
    public class TokenService : ServiceBase
    {
        private readonly UserManager _userManager;

        public TokenService(IDataContextFactory dataContextFactory) : base(dataContextFactory)
        {
            _userManager = new UserManager(dataContextFactory);
        }

        public async Task AddTokensToUser(ApplicationUser user, int? tokens, ApplicationUser modifiedByUser, string userHostAddress, string usage)
        {
            if (!tokens.HasValue)
            {
                return;
            }

            user = await _userManager.FindByIdAsync(user.Id);

            var originalTokenAmount = user.Tokens;
            user.Tokens += tokens.Value;

            await _userManager.UpdateAsync(user);

            using (var dc = DataContext())
            {
                dc.TokenLogs.Add(new TokenLog
                {
                    UserId = user.Id,
                    ModifiedByUserId = modifiedByUser.Id,
                    OriginalTokenAmount = originalTokenAmount,
                    ModifiedTokenAmount = user.Tokens,
                    UserHostAddress = userHostAddress,
                    Usage = usage
                });

                await dc.SaveChangesAsync();
            }
        }

        public async Task RemoveTokensFromUser(ApplicationUser user, int? tokens, ApplicationUser modifiedByUser, string userHostAddress, string usage)
        {
            if (!tokens.HasValue)
            {
                return;
            }

            tokens = tokens * -1;

            await AddTokensToUser(user, tokens, modifiedByUser, userHostAddress, usage);
        }

        /// <summary>
        /// Gets a collection of token logs.
        /// </summary>
        public async Task<IReadOnlyCollection<TokenLog>> GetTokenLogs()
        {
            using (var dc = DataContext())
            {
                return await dc
                    .TokenLogs
                    .Include(i => i.User)
                    .Include(i => i.ModifiedByUser)
                    .OrderBy(i => i.CreatedUtc)
                    .ToListAsync();
            }
        }

        /// <summary>
        /// Gets a collection of token logs by user ID.
        /// </summary>
        public async Task<IReadOnlyCollection<TokenLog>> GetTokenLogsByUserId(string userId)
        {
            using (var dc = DataContext())
            {
                return await dc
                    .TokenLogs
                    .Include(i => i.User)
                    .Include(i => i.ModifiedByUser)
                    .Where(i => i.UserId == userId)
                    .OrderBy(i => i.CreatedUtc)
                    .ToListAsync();
            }
        }
    }
}
