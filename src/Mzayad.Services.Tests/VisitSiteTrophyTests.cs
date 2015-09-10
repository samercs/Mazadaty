using Mzayad.Models;
using Mzayad.Services.Tests.Fakes;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Mzayad.Services.Tests
{
    [TestFixture]
    public class VisitSiteTrophyTests
    {
        private static SessionLogService GetSessionLogService(IEnumerable<SessionLog> sessions)
        {
            var dc = new InMemoryDataContext();
            foreach (var session in sessions)
            {
                dc.SessionLogs.Add(session);
            }

            var sessionLogService = new SessionLogService(new InMemoryDataContextFactory(dc));
            return sessionLogService;
        }

        [Test]
        public void VisitSite_Before30Days()
        {
            var sessionLogService = GetSessionLogService(new[]
            {
                new SessionLog {UserId = Constants.AnyUserId, CreatedUtc = DateTime.UtcNow.AddDays(-32)},
                new SessionLog {UserId = Constants.AnyUserId, CreatedUtc = DateTime.UtcNow.AddDays(-30)},
                new SessionLog {UserId = Constants.AnyUserId, CreatedUtc = DateTime.UtcNow}
            });

            var result = sessionLogService.GetLastVisitBeforeToday(Constants.AnyUserId).CreatedUtc;
            Assert.GreaterOrEqual(DateTime.UtcNow.Date, result.AddDays(30));
        }

        [Test]
        public void VisitSite_Before29Days()
        {
            var sessionLogService = GetSessionLogService(new[]
            {
                new SessionLog {UserId = Constants.AnyUserId, CreatedUtc = DateTime.UtcNow.AddDays(-32)},
                new SessionLog {UserId = Constants.AnyUserId, CreatedUtc = DateTime.UtcNow.AddDays(-20)},
                new SessionLog {UserId = Constants.AnyUserId, CreatedUtc = DateTime.UtcNow}
            });

            var result = sessionLogService.GetLastVisitBeforeToday(Constants.AnyUserId).CreatedUtc;
            Assert.LessOrEqual(DateTime.UtcNow.Date, result.AddDays(29));
        }
    }
}
