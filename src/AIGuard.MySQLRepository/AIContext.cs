using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace AIGuard.MySQLRepository
{
    public class AIContext : DbContext
    {
        private string _connectionStr;

        public AIContext(string connectionStr)
        {
            _connectionStr = connectionStr;
        }
        public DbSet<Capture> Captures { get; set; }
        public DbSet<Detection> Detections { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySQL(_connectionStr);
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Capture>(
                    c => c.ToTable("captures")
                );

            modelBuilder.Entity<Capture>(
                e => e.HasMany(d => d.Detections));

            modelBuilder.Entity<Detection>(
                    d => d.ToTable("detections")
                );



            base.OnModelCreating(modelBuilder);
        }
    }
}
