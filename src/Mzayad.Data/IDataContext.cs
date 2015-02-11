using System;
using System.Data.Entity;
using System.Threading.Tasks;
using Mzayad.Models;

namespace Mzayad.Data
{
    public interface IDataContext : IDisposable
    {
        // TODO add IDbSets
        IDbSet<EmailTemplate> EmailTemplates { get; set; } 


        int SaveChanges();
        Task<int> SaveChangesAsync();
        void SetModified(object entity);

    }
}
