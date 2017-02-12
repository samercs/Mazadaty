﻿using Mzayad.Core.Exceptions;
using Mzayad.Data;
using Mzayad.Models;
using Mzayad.Models.Enums;
using OrangeJetpack.Localization;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Mzayad.Services
{
    public class PrizeService : ServiceBase
    {
        public PrizeService(IDataContextFactory dataContextFactory) : base(dataContextFactory)
        {
        }

        public async Task<IEnumerable<Prize>> GetAll(string languageCode = "en", string search = "")
        {
            using (var dc = DataContext())
            {
                var query = dc.Prizes.AsQueryable();
                if (!string.IsNullOrEmpty(search))
                {
                    query = query.Where(i => i.Title.Contains(search));
                }
                var result = await query.ToListAsync();
                return result.Localize<Prize>(languageCode, i => i.Title);
            }
        }

        public async Task<Prize> Add(Prize prize)
        {
            using (var dc = DataContext())
            {
                dc.Prizes.Add(prize);
                await dc.SaveChangesAsync();
                return prize;
            }
        }

        public async Task<Prize> GetById(int id)
        {
            using (var dc = DataContext())
            {
                return await dc.Prizes.SingleOrDefaultAsync(i => i.PrizeId == id);
            }

        }

        public async Task<Prize> Save(Prize prize)
        {
            using (var dc = DataContext())
            {
                dc.SetModified(prize);
                await dc.SaveChangesAsync();
                return prize;
            }
        }

        public async Task<IEnumerable<Prize>> GetAvaliablePrize(string languageCode = "en")
        {
            using (var dc = DataContext())
            {
                var query = dc.Prizes
                    .Where(i => i.Limit > 0 || !i.Limit.HasValue)
                    .Where(i => i.Status == PrizeStatus.Public)
                    .OrderByDescending(i => i.Weight);
                var result = await query.ToListAsync();
                return result.Localize<Prize>(languageCode, i => i.Title);
            }
        }

        public async Task<Prize> GetRandomPrize()
        {
            Random rnd = new Random();

            using (var dc = DataContext())
            {
                var query = dc.Prizes
                    .Where(i => i.Limit > 0 || !i.Limit.HasValue)
                    .Where(i => i.Status == PrizeStatus.Public)
                    .OrderByDescending(i => i.Weight);
                var data = await query.ToListAsync();
                var totalWeight = data.Sum(i => (int)(i.Weight * 100));
                int randomNumber = rnd.Next(0, totalWeight);
                Prize selectedPrize = null;
                foreach (Prize prize in data)
                {
                    if (randomNumber <= prize.Weight * 100)
                    {
                        selectedPrize = prize;
                        break;
                    }
                    randomNumber = randomNumber - (int)(prize.Weight * 100);
                }
                return selectedPrize;
            }

        }

        public async Task<UserPrizeLog> LogUserPrize(UserPrizeLog userPrizeLog, int prizeId, bool isComplete)
        {
            using (var dc = DataContext())
            {
                userPrizeLog.PrizeId = prizeId;
                userPrizeLog.IsComplete = isComplete;
                dc.SetModified(userPrizeLog);
                await dc.SaveChangesAsync();
                return userPrizeLog;
            }
        }


        public async Task<bool> UserHasFreeAvatar(ApplicationUser user)
        {
            using (var dc = DataContext())
            {
                var userPrize = await dc.UserPrizeLogs
                    .Where(i => i.IsComplete == false)
                    .Where(i => i.UserId == user.Id)
                    .Where(i => i.Prize.PrizeType == PrizeType.Avatar).ToListAsync();
                return userPrize.Any();
            }
        }

        public async Task UpdateUserHasFreeAvatar(ApplicationUser user)
        {
            using (var dc = DataContext())
            {
                var userPrizes = await dc.UserPrizeLogs
                    .Where(i => i.IsComplete == false)
                    .Where(i => i.UserId == user.Id)
                    .Where(i => i.Prize.PrizeType == PrizeType.Avatar).ToListAsync();
                var userPrize = userPrizes.FirstOrDefault();
                if (userPrize == null)
                {
                    throw new UserNotHaveAvatarPrize("User does not have avatar prize.");
                }

                userPrize.IsComplete = true;
                dc.SetModified(userPrize);
                await dc.SaveChangesAsync();
            }
        }

        public async Task<UserPrizeLog> InsertUserPrize(ApplicationUser user)
        {
            using (var dc = DataContext())
            {
                var prizeLog = new UserPrizeLog
                {
                    UserId = user.Id
                };
                dc.UserPrizeLogs.Add(prizeLog);
                await dc.SaveChangesAsync();
                return prizeLog;
            }
        }

        public async Task<UserPrizeLog> GetPrizeLogById(int id)
        {
            using (var dc = DataContext())
            {
                return await dc.UserPrizeLogs.SingleOrDefaultAsync(i => i.UserPrizeLogId == id);
            }
        }

        public async Task<UserPrizeLog> GetUserAvilablePrize(ApplicationUser user)
        {
            using (var dc = DataContext())
            {
                return await dc.UserPrizeLogs.FirstOrDefaultAsync(i => i.UserId == user.Id && !i.PrizeId.HasValue);
            }

        }
    }
}