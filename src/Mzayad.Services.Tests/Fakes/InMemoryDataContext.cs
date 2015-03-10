using System.Data.Entity;
using System.Threading.Tasks;
using Mzayad.Data;
using Mzayad.Models;

namespace Mzayad.Services.Tests.Fakes
{
    public class InMemoryDataContext : IDataContext
    {
        public IDbSet<Address> Addresses { get; set; }
        public IDbSet<EmailTemplate> EmailTemplates { get; set; }
        public IDbSet<Category> Categories { get; set; }
        public IDbSet<Product> Products { get; set; }
        public IDbSet<ProductImage> ProductImages { get; set; }
        public IDbSet<Auction> Auctions { get; set; }
        public IDbSet<Specification> Specifications { get; set; }
        public IDbSet<CategoryNotification> CategoryNotifications { get; set; }

        public InMemoryDataContext()
        {
            Categories = new TestDbSet<Category>();
            CategoryNotifications = new TestDbSet<CategoryNotification>();
        }

        public int SaveChangesCount { get; private set; }
        public int SaveChanges()
        {
            SaveChangesCount++;
            return 1;
        }

        public Task<int> SaveChangesAsync()
        {
            SaveChangesCount++;
            return Task<int>.Factory.StartNew(() => 1);
        }
        
        public void SetModified(object entity)
        {
            // do nothing
        }

        public void Dispose()
        {
            // do nothing
        }
    }
}
