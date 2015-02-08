using System;
using System.Threading.Tasks;

namespace Mzayad.Data
{
    public interface IDataContext : IDisposable
    {
        // TODO add IDbSets

        int SaveChanges();
        Task<int> SaveChangesAsync();
        void SetModified(object entity);
    }
}
