using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace WebApplication2.Model
{
    public partial class intenettaxiContext : DbContext
    {
        public intenettaxiContext()
        {
        }

        public intenettaxiContext(DbContextOptions<intenettaxiContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Datum> Data { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=intenet taxi;Integrated Security=True");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Datum>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("data");

                entity.Property(e => e.Base).HasMaxLength(50);

                entity.Property(e => e.DateTime).HasColumnName("Date_Time");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
