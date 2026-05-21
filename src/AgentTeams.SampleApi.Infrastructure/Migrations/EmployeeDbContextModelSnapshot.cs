using System;
using AgentTeams.SampleApi.Infrastructure.Employees;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace AgentTeams.SampleApi.Infrastructure.Migrations;

[DbContext(typeof(EmployeeDbContext))]
partial class EmployeeDbContextModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder modelBuilder)
    {
#pragma warning disable 612, 618
        modelBuilder.HasAnnotation("ProductVersion", "10.0.8");

        modelBuilder.Entity("AgentTeams.SampleApi.Domain.Employees.Employee", b =>
        {
            b.Property<Guid>("Id")
                .ValueGeneratedOnAdd()
                .HasColumnType("TEXT")
                .HasColumnName("id");

            b.Property<DateTimeOffset>("CreatedAt")
                .HasColumnType("TEXT")
                .HasColumnName("created_at");

            b.Property<string>("Department")
                .IsRequired()
                .HasMaxLength(120)
                .HasColumnType("TEXT")
                .HasColumnName("department");

            b.Property<string>("Email")
                .IsRequired()
                .HasMaxLength(256)
                .HasColumnType("TEXT")
                .HasColumnName("email");

            b.Property<string>("EmployeeCode")
                .IsRequired()
                .HasMaxLength(32)
                .HasColumnType("TEXT")
                .HasColumnName("employee_code");

            b.Property<string>("FullName")
                .IsRequired()
                .HasMaxLength(160)
                .HasColumnType("TEXT")
                .HasColumnName("full_name");

            b.Property<bool>("IsActive")
                .HasColumnType("INTEGER")
                .HasColumnName("is_active");

            b.Property<string>("JobTitle")
                .IsRequired()
                .HasMaxLength(120)
                .HasColumnType("TEXT")
                .HasColumnName("job_title");

            b.Property<DateTimeOffset>("UpdatedAt")
                .HasColumnType("TEXT")
                .HasColumnName("updated_at");

            b.HasKey("Id")
                .HasName("pk_employees");

            b.HasIndex("EmployeeCode")
                .IsUnique()
                .HasDatabaseName("ix_employees_employee_code");

            b.ToTable("employees", (string)null);
        });
#pragma warning restore 612, 618
    }
}
