using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mzayad.Data;
using Mzayad.Models;

namespace Mzayad.Services
{
    public class SpecificationService : ServiceBase
    {
        public SpecificationService(IDataContextFactory dataContextFactory) : base(dataContextFactory)
        {
        }

        public async Task<IEnumerable<Specification>> GetAll()
        {
            using (var dc=DataContext())
            {
                return await dc.Specifications.ToArrayAsync();
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
    }
}
