using AgentTeams.SampleApi.Domain.Employees;
using Microsoft.EntityFrameworkCore;

namespace AgentTeams.SampleApi.Infrastructure.Employees;

public sealed class EmployeeDbContext(DbContextOptions<EmployeeDbContext> options) : DbContext(options)
{
    public DbSet<Employee> Employees => Set<Employee>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(EmployeeDbContext).Assembly);
    }
}
