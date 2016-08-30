using Mzayad.Data;
using Mzayad.Models;
using OrangeJetpack.Localization;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Mzayad.Services
{
    public class SpecificationService : ServiceBase
    {
        public SpecificationService(IDataContextFactory dataContextFactory) : base(dataContextFactory)
        {
        }

        public async Task<IEnumerable<Specification>> GetAll()
        {
            using (var dc = DataContext())
            {
                return await dc.Specifications.OrderBy(i => i.Name).ToArrayAsync();
            }
        }

        public async Task<IEnumerable<Specification>> GetAll(string languageCode)
        {
            using (var dc = DataContext())
            {
                var result = await dc.Specifications.OrderBy(i => i.Name).ToListAsync();
                
                return result.Localize(languageCode, i => i.Name);
            }
        }

        public async Task<Specification> Add(Specification specification)
        {
            using (var dc=DataContext())
            {
                dc.Specifications.Add(specification);
                await dc.SaveChangesAsync();
                return specification;
            }
        }

        public async Task<Specification> GetById(int id)
        {
            using (var dc=DataContext())
            {
                return await dc.Specifications.SingleOrDefaultAsync(i => i.SpecificationId == id);
            }
        }

        public async Task<Specification> Delete(Specification specification)
        {
            using (var dc=DataContext())
            {
                dc.Specifications.Attach(specification);
                dc.Specifications.Remove(specification);
                await dc.SaveChangesAsync();
                return specification;
            }
        }

        public async Task<Specification> Update(Specification specification)
        {
            using (var dc=DataContext())
            {
                dc.SetModified();
                await dc.SaveChangesAsync();
                return specification;
            }
        }
    }
}
