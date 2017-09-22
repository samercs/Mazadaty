using Mzayad.Models;
using Mzayad.Services;
using Mzayad.Web.Core.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mzayad.Web.Models.User
{
    public class NotificationModelView
    {
        public IEnumerable<int> SelectedCategories { get; set; }
        public IEnumerable<Category> Categories { get; set; }
        public IEnumerable<CategoryNotification> CategoryNotification { get; set; }
        public string UserId { get; set; }
        public bool CreateNew { get; set; }
        public  bool AutoBidNotification { get; set; }

        public async Task<NotificationModelView> Hydrate(IAuthService authService, CategoryService categoryService,NotificationService notificationService,string languageCode)
        {
            UserId = authService.CurrentUserId();
            CategoryNotification = await notificationService.GetByUser(UserId);
            
            Categories = await categoryService.GetCategoriesAsHierarchy(languageCode);
            SelectedCategories = CategoryNotification.Select(i => i.CategoryId).ToList();

            return this;
        }
    }
}