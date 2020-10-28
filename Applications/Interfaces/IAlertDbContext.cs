using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Applications.Interfaces
{
    public interface IAlertDbContext
    {
        DbSet<User> User { get; set; }
        DbSet<Transaction> Transaction { get; set; }
        int SaveChanges();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
