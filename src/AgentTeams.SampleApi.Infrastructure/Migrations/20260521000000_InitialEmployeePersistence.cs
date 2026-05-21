using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgentTeams.SampleApi.Infrastructure.Migrations;

public partial class InitialEmployeePersistence : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "employees",
            columns: table => new
            {
                id = table.Column<Guid>(type: "TEXT", nullable: false),
                employee_code = table.Column<string>(type: "TEXT", maxLength: 32, nullable: false),
                full_name = table.Column<string>(type: "TEXT", maxLength: 160, nullable: false),
                email = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                department = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                job_title = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                is_active = table.Column<bool>(type: "INTEGER", nullable: false),
                created_at = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                updated_at = table.Column<DateTimeOffset>(type: "TEXT", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_employees", employee => employee.id);
            });

        migrationBuilder.CreateIndex(
            name: "ix_employees_employee_code",
            table: "employees",
            column: "employee_code",
            unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "employees");
    }
}
