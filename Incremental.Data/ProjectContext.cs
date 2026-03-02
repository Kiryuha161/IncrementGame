using Incremental.Data.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incremental.Data
{
    public class ProjectContext : DbContext
    {
        public ProjectContext(DbContextOptions<ProjectContext> options) : base(options) { }

        public DbSet<Point> Points { get; set; }
        public DbSet<Upgrade> Upgrades { get; set; }
        public DbSet<PlayerUpgrade> PlayerUpgrades { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<PlayerUpgrade>()
                .HasIndex(pu => pu.PointsId)
                .HasDatabaseName("IX_PlayerUpgrades_PointsId");

            modelBuilder.Entity<PlayerUpgrade>()
                .HasIndex(pu => pu.UpgradeId)
                .HasDatabaseName("IX_PlayerUpgrades_UpgradeId");
        }
    }
}

