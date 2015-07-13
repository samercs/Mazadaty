using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Mzayad.Data;
using Mzayad.Models;
using Mzayad.Services.Identity;
using System.Threading.Tasks;

namespace Mzayad.Services
{
    public class TokenService : ServiceBase
    {
        private readonly UserManager _userManager;
        
        public TokenService(IDataContextFactory dataContextFactory) : base(dataContextFactory)
        {
            _userManager = new UserManager(dataContextFactory);
        }

        public async Task AddTokensToUser(ApplicationUser user, int? tokens, ApplicationUser modifiedByUser, string userHostAddress)
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
                    UserHostAddress = userHostAddress
                });

                await dc.SaveChangesAsync();
            }
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
