using ExpressionGeneratorApp.Entities;
using Microsoft.EntityFrameworkCore;

namespace ExpressionGeneratorApp;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions options)
        : base(options) 
    {
        this.ChangeTracker.LazyLoadingEnabled = false;
    }

    public DbSet<Employee> Employees { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<Team> Teams { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<Company> Companies { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder
            .Entity<Team>()
            .HasOne(e => e.Department)
            .WithMany(d => d.Teams)
            .HasForeignKey(e => e.DepartmentId)
            .HasConstraintName("FK_Team_Department")
            .IsRequired();

        modelBuilder
            .Entity<Project>()
            .HasOne(p => p.Team)
            .WithMany(t => t.Projects)
            .HasForeignKey(e => e.TeamId)
            .HasConstraintName("FK_Team_Project")
            .IsRequired();

        modelBuilder
            .Entity<Employee>()
            .HasOne(p => p.Team)
            .WithMany(t => t.Employees)
            .HasForeignKey(e => e.TeamId)
            .HasConstraintName("FK_Team_Employee")
            .IsRequired();

        modelBuilder
            .Entity<Department>()
            .HasOne(p => p.Company)
            .WithMany(t => t.Departments)
            .HasForeignKey(e => e.CompanyId)
            .HasConstraintName("FK_Department_Company")
            .IsRequired();

        base.OnModelCreating(modelBuilder);
    }
}
