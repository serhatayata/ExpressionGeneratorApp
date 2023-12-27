using ExpressionGeneratorApp.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpressionGeneratorApp;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions options)
        : base(options) { }

    public DbSet<Employee> Employees { get; set; }
}
