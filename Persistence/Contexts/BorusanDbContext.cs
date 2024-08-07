﻿using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Contexts
{
    // 10:10
    public class BorusanDbContext : DbContext
    {
        public DbSet<Car> Cars { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<OperationClaim> OperationClaims { get; set; }
        public DbSet<UserOperationClaim> UserOperationClaims { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB; Database=BorusanDB; Trusted_Connection=True;");
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Car>().ToTable("Cars").HasKey(c => c.Id);

            modelBuilder.Entity<Car>()
                .HasOne(i => i.Brand)
                .WithMany(b => b.Cars)
                .HasForeignKey(c => c.BrandId);

            //modelBuilder.Entity<UserOperationClaim>().HasOne(i => i.User).WithMany(i => i.UserOperationClaims).HasForeignKey(i => i.Abc);

            base.OnModelCreating(modelBuilder);
        }
    }
}
