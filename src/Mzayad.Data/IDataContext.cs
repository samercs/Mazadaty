﻿using System;
using System.Data.Entity;
using System.Threading.Tasks;
using Mzayad.Models;

namespace Mzayad.Data
{
    public interface IDataContext : IDisposable
    {
        IDbSet<Address> Addresses { get; set; }
        IDbSet<EmailTemplate> EmailTemplates { get; set; }
        IDbSet<Category> Categories { get; set; }
        IDbSet<Product> Products { get; set; }
        IDbSet<ProductImage> ProductImages { get; set; } 

        int SaveChanges();
        Task<int> SaveChangesAsync();
        void SetModified(object entity);

    }
}
