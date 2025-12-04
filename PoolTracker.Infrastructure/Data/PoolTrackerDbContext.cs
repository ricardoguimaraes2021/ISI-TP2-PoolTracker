using Microsoft.EntityFrameworkCore;
using PoolTracker.Core.Entities;

namespace PoolTracker.Infrastructure.Data;

public class PoolTrackerDbContext : DbContext
{
    public PoolTrackerDbContext(DbContextOptions<PoolTrackerDbContext> options) : base(options)
    {
    }

    public DbSet<PoolStatus> PoolStatus { get; set; }
    public DbSet<Worker> Workers { get; set; }
    public DbSet<ActiveWorker> ActiveWorkers { get; set; }
    public DbSet<DailyVisitor> DailyVisitors { get; set; }
    public DbSet<WaterQuality> WaterQuality { get; set; }
    public DbSet<Cleaning> Cleanings { get; set; }
    public DbSet<DailyReport> DailyReports { get; set; }
    public DbSet<ShoppingItem> ShoppingList { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // PoolStatus
        modelBuilder.Entity<PoolStatus>(entity =>
        {
            entity.ToTable("pool_status");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.CurrentCount).HasDefaultValue(0);
            entity.Property(e => e.MaxCapacity).HasDefaultValue(120);
            entity.Property(e => e.IsOpen).HasDefaultValue(true);
            entity.Property(e => e.LocationName).HasMaxLength(255).HasDefaultValue("Piscina Municipal da Sobreposta");
            entity.Property(e => e.Address).HasMaxLength(255).HasDefaultValue("R. da Piscina 22, 4715-553 Sobreposta");
            entity.Property(e => e.Phone).HasMaxLength(50).HasDefaultValue("253 636 948");
            entity.HasIndex(e => e.IsOpen);
            entity.HasIndex(e => e.LastUpdated);
        });

        // Worker
        modelBuilder.Entity<Worker>(entity =>
        {
            entity.ToTable("workers");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.WorkerId).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Name).HasMaxLength(255).IsRequired();
            entity.Property(e => e.Role).HasConversion<string>().HasMaxLength(50);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.HasIndex(e => e.WorkerId).IsUnique();
            entity.HasIndex(e => e.Role);
            entity.HasIndex(e => e.IsActive);
        });

        // ActiveWorker
        modelBuilder.Entity<ActiveWorker>(entity =>
        {
            entity.ToTable("active_workers");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.WorkerId).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Role).HasConversion<string>().HasMaxLength(50);
            entity.Property(e => e.ShiftType).HasConversion<string>().HasMaxLength(10);
            entity.HasIndex(e => e.WorkerId);
            entity.HasIndex(e => e.Role);
            entity.HasIndex(e => e.StartTime);
            entity.HasIndex(e => e.ShiftType);
            
            entity.HasOne(e => e.Worker)
                .WithMany(w => w.ActiveWorkers)
                .HasForeignKey(e => e.WorkerId)
                .HasPrincipalKey(w => w.WorkerId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // DailyVisitor
        modelBuilder.Entity<DailyVisitor>(entity =>
        {
            entity.ToTable("daily_visitors");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.VisitDate).IsRequired();
            entity.Property(e => e.TotalVisitors).HasDefaultValue(0);
            entity.HasIndex(e => e.VisitDate).IsUnique();
        });

        // WaterQuality
        modelBuilder.Entity<WaterQuality>(entity =>
        {
            entity.ToTable("water_quality");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.PoolType).HasConversion<string>().HasMaxLength(20);
            entity.Property(e => e.PhLevel).HasPrecision(4, 2);
            entity.Property(e => e.Temperature).HasPrecision(5, 2);
            entity.HasIndex(e => e.PoolType);
            entity.HasIndex(e => e.MeasuredAt);
        });

        // Cleaning
        modelBuilder.Entity<Cleaning>(entity =>
        {
            entity.ToTable("cleanings");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.CleaningType).HasConversion<string>().HasMaxLength(20);
            entity.HasIndex(e => e.CleanedAt);
            entity.HasIndex(e => e.CleaningType);
        });

        // DailyReport
        modelBuilder.Entity<DailyReport>(entity =>
        {
            entity.ToTable("daily_reports");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.ReportDate).IsRequired();
            entity.Property(e => e.TotalVisitors).HasDefaultValue(0);
            entity.Property(e => e.MaxOccupancy).HasDefaultValue(0);
            entity.Property(e => e.AvgOccupancy).HasPrecision(5, 2);
            // Removed SQL Server-specific nvarchar(max) column types for SQLite compatibility
            entity.HasIndex(e => e.ReportDate).IsUnique();
        });

        // ShoppingItem
        modelBuilder.Entity<ShoppingItem>(entity =>
        {
            entity.ToTable("shopping_list");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Name).HasMaxLength(255).IsRequired();
            entity.Property(e => e.Category).HasConversion<string>().HasMaxLength(20);
            entity.HasIndex(e => e.Category);
        });
    }
}

