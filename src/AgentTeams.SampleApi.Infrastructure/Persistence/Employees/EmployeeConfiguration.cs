using AgentTeams.SampleApi.Domain.Employees;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AgentTeams.SampleApi.Infrastructure.Persistence.Employees;

public sealed class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.ToTable("employees");

        builder.HasKey(employee => employee.Id)
            .HasName("pk_employees");

        builder.Property(employee => employee.Id)
            .HasColumnName("id");

        builder.Property(employee => employee.EmployeeCode)
            .HasColumnName("employee_code")
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(employee => employee.FullName)
            .HasColumnName("full_name")
            .HasMaxLength(160)
            .IsRequired();

        builder.Property(employee => employee.Email)
            .HasColumnName("email")
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(employee => employee.Department)
            .HasColumnName("department")
            .HasMaxLength(120)
            .IsRequired();

        builder.Property(employee => employee.JobTitle)
            .HasColumnName("job_title")
            .HasMaxLength(120)
            .IsRequired();

        builder.Property(employee => employee.IsActive)
            .HasColumnName("is_active")
            .IsRequired();

        builder.Property(employee => employee.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(employee => employee.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();

        builder.HasIndex(employee => employee.EmployeeCode)
            .IsUnique()
            .HasDatabaseName("ix_employees_employee_code");
    }
}
