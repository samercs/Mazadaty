using System.Linq;
using System.Threading.Tasks;
using Mzayad.Models;
using Mzayad.Services.Tests.Fakes;
using NUnit.Framework;

namespace Mzayad.Services.Tests
{
    // ReSharper disable InconsistentNaming
    // ReSharper disable PossibleNullReferenceException

    [TestFixture]
    public class NotificationServiceTests
    {
        [Test]
        public async Task GetByCategoryId_WithCategoryIds_ReturnsCorrectList()
        {
            var dc = new InMemoryDataContext();
            dc.CategoryNotifications.Add(new CategoryNotification {CategoryId = 1});
            dc.CategoryNotifications.Add(new CategoryNotification {CategoryId = 2});
            dc.CategoryNotifications.Add(new CategoryNotification {CategoryId = 3});
            dc.CategoryNotifications.Add(new CategoryNotification {CategoryId = 4});
            dc.CategoryNotifications.Add(new CategoryNotification {CategoryId = 5});

            var service = new NotificationService(new InMemoryDataContextFactory(dc));
            var notifications = (await service.GetByCategoryId(new[] {1, 2, 3, 111, 222, 333})).ToList();

            Assert.AreEqual(3, notifications.Count());
            CollectionAssert.Contains(notifications.Select(i => i.CategoryId), 1);
            CollectionAssert.DoesNotContain(notifications.Select(i => i.CategoryId), 111);
        }
    }

    // ReSharper restore InconsistentNaming
    // ReSharper restore PossibleNullReferenceException
}