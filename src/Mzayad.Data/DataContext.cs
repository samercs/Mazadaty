﻿using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;
using Mzayad.Models;

namespace Mzayad.Data
{
    public class DataContext : IdentityDbContext<ApplicationUser>, IDataContext
    {
        public IDbSet<Address> Addresses { get; set; }
        public IDbSet<EmailTemplate> EmailTemplates { get; set; } 

        public DataContext(): base("DefaultConnection")
        {
            Configuration.ProxyCreationEnabled = false;
            Configuration.LazyLoadingEnabled = false;
        }

        public static DataContext Create()
        {
            return new DataContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Properties<decimal>().Configure(prop => prop.HasPrecision(18, 3));
            
            base.OnModelCreating(modelBuilder);
        }

        public void SetModified(object entity)
        {
            Entry(entity).State = EntityState.Modified;
        }

        public override int SaveChanges()
        {
            try
            {
                return base.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                TraceValidationErrors(ex);
                throw;
            }
        }

        public override Task<int> SaveChangesAsync()
        {
            try
            {
                return base.SaveChangesAsync();
            }
            catch (DbEntityValidationException ex)
            {
                TraceValidationErrors(ex);
                throw;
            }
        }

        private static void TraceValidationErrors(DbEntityValidationException ex)
        {
            foreach (var validationErrors in ex.EntityValidationErrors)
            {
                foreach (var validationError in validationErrors.ValidationErrors)
                {
                    Trace.TraceError("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                }
            }
        }
    }
}
