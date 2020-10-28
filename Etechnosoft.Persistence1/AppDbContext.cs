using System;
using Applications.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;


namespace Persistence
{

    public class AppDbContext : DbContext, IAlertDbContext

    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
        public DbSet<User> User { get; set; }
        public DbSet<Transaction> Transaction { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);
        }
    }
}
