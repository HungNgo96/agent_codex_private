---
name: database-migration-dotnet
description: >-
  Database migration best practices for .NET applications using EF Core, including schema changes,
  data migrations, rollbacks, and zero-downtime deployment strategies. Use when creating or altering
  database tables, adding/removing columns or indexes, running data migrations, or planning 
  zero-downtime schema changes.
---

# Database Migration Patterns (.NET / EF Core)

Safe, reversible database schema changes for production systems using Entity Framework Core.

## When to Activate

- Creating or altering database tables
- Adding/removing columns or indexes
- Running data migrations (backfill, transform)
- Planning zero-downtime schema changes
- Setting up migration tooling for a new project

## Core Principles

1. **Every change is a migration** — never alter production databases manually
2. **Migrations are forward-only in production** — rollbacks use new forward migrations
3. **Schema and data migrations are separate** — never mix DDL and DML in one migration
4. **Test migrations against production-sized data** — a migration that works on 100 rows may lock on 10M
5. **Migrations are immutable once deployed** — never edit a migration that has run in production

## Migration Safety Checklist

Before applying any migration:

- [ ] Migration has both UP and DOWN (or is explicitly marked irreversible)
- [ ] No full table locks on large tables (use concurrent operations)
- [ ] New columns have defaults or are nullable
- [ ] Indexes created concurrently
- [ ] Data backfill is a separate migration from schema change
- [ ] Tested against a copy of production data
- [ ] Rollback plan documented

## EF Core Migrations

### Basic Workflow

```bash
# Install EF Core tools if not already
dotnet tool install --global dotnet-ef

# Create initial migration
dotnet ef migrations add InitialCreate

# Add a new migration
dotnet ef migrations add AddUserAvatar

# Apply pending migrations
dotnet ef database update

# Remove last migration (if not applied)
dotnet ef migrations remove

# Generate SQL script for review before applying
dotnet ef migrations script --idempotent

# Generate SQL script with specific from/to versions
dotnet ef migrations script AddUserAvatar UpdateUserAvatar -o migrate.sql
```

### Creating Migrations with EF Core

```csharp
// When you modify your DbContext or entity classes
public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public DateTime CreatedAt { get; set; }
}

// Create migration (EF Core generates this automatically)
public partial class AddUserAvatar : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "avatar_url",
            table: "users",
            type: "text",
            nullable: true);

        migrationBuilder.CreateIndex(
            name: "ix_users_email",
            table: "users",
            column: "email",
            unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(name: "ix_users_email", table: "users");
        migrationBuilder.DropColumn(name: "avatar_url", table: "users");
    }
}
```

## PostgreSQL Patterns

### Adding a Column Safely

```csharp
// GOOD: Nullable column, no lock
migrationBuilder.AddColumn<string>(
    name: "avatar_url",
    table: "users",
    type: "text",
    nullable: true);

// GOOD: Column with default (EF Core handles this)
migrationBuilder.AddColumn<bool>(
    name: "is_active",
    table: "users",
    type: "boolean",
    nullable: false,
    defaultValue: true);

// BAD: NOT NULL without default on existing table
// This locks the table and rewrites every row
migrationBuilder.AddColumn<string>(
    name: "role",
    table: "users",
    type: "text",
    nullable: false); // Don't do this without a default!
```

### Adding an Index Without Downtime

```csharp
// Use Sql() for concurrent index creation
protected override void Up(MigrationBuilder migrationBuilder)
{
    // BAD: Blocks writes on large tables
    migrationBuilder.CreateIndex(
        name: "ix_users_email",
        table: "users",
        column: "email");

    // GOOD: Non-blocking (PostgreSQL only)
    migrationBuilder.Sql(
        "CREATE INDEX CONCURRENTLY idx_users_email ON users (email);");
    
    // Note: CONCURRENTLY cannot run inside a transaction
    // EF Core migrations wrap everything in a transaction by default
    // You need to disable transaction for this migration
}

// Disable transaction wrapping for concurrent index
protected override void Up(MigrationBuilder migrationBuilder)
{
    migrationBuilder.Sql(@"
        CREATE INDEX CONCURRENTLY IF NOT EXISTS idx_users_email 
        ON users (email);
    ", suppressTransaction: true);
}
```

### Renaming a Column (Zero-Downtime)

Never rename directly in production. Use the expand-contract pattern:

```csharp
// Step 1: Add new column (Migration 001)
protected override void Up(MigrationBuilder migrationBuilder)
{
    migrationBuilder.AddColumn<string>(
        name: "display_name",
        table: "users",
        type: "character varying(100)",
        nullable: true);
}

// Step 2: Backfill data (Migration 002 - Data Migration)
protected override void Up(MigrationBuilder migrationBuilder)
{
    migrationBuilder.Sql(@"
        UPDATE users 
        SET display_name = username 
        WHERE display_name IS NULL;
    ");
}

// Step 3: Update application code to read/write both columns
// Deploy application changes

// Step 4: Stop writing to old column, drop it (Migration 003)
protected override void Up(MigrationBuilder migrationBuilder)
{
    migrationBuilder.DropColumn(name: "username", table: "users");
}
```

### Removing a Column Safely

```csharp
// Step 1: Remove all application references to the column
// Step 2: Deploy application without the column reference
// Step 3: Drop column in next migration
protected override void Up(MigrationBuilder migrationBuilder)
{
    migrationBuilder.DropColumn(name: "legacy_status", table: "orders");
}
```

## Large Data Migrations

### Batch Update Pattern

```csharp
// Data migration for large tables
protected override void Up(MigrationBuilder migrationBuilder)
{
    migrationBuilder.Sql(@"
        DO $$
        DECLARE
            batch_size INT := 10000;
            rows_updated INT;
        BEGIN
            LOOP
                UPDATE users
                SET normalized_email = LOWER(email)
                WHERE id IN (
                    SELECT id FROM users
                    WHERE normalized_email IS NULL
                    LIMIT batch_size
                    FOR UPDATE SKIP LOCKED
                );
                GET DIAGNOSTICS rows_updated = ROW_COUNT;
                RAISE NOTICE 'Updated % rows', rows_updated;
                EXIT WHEN rows_updated = 0;
                COMMIT;
            END LOOP;
        END $$;
    ");
}
```

### Split Large Table Operations

```csharp
// Instead of one big operation, split by time chunks
protected override void Up(MigrationBuilder migrationBuilder)
{
    // Process users created in batches by date
    migrationBuilder.Sql(@"
        DO $$
        DECLARE
            min_date DATE;
            max_date DATE;
            current_date DATE;
        BEGIN
            SELECT MIN(created_at)::DATE, MAX(created_at)::DATE 
            INTO min_date, max_date 
            FROM users;
            
            current_date := min_date;
            WHILE current_date <= max_date LOOP
                UPDATE users
                SET display_name = first_name || ' ' || last_name
                WHERE DATE(created_at) = current_date
                  AND display_name IS NULL;
                
                current_date := current_date + INTERVAL '1 day';
            END LOOP;
        END $$;
    ");
}
```

## EF Core IEntityTypeConfiguration

### Separate Configuration Classes

```csharp
// Entity
public class Order
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public OrderStatus Status { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Customer Customer { get; set; } = null!;
    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
}

// Configuration (IEntityTypeConfiguration)
public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("orders");

        builder.HasKey(o => o.Id);

        builder.Property(o => o.Id)
            .HasColumnName("id")
            .HasColumnType("uuid");

        builder.Property(o => o.Status)
            .HasColumnName("status")
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(o => o.TotalAmount)
            .HasColumnName("total_amount")
            .HasColumnType("decimal(18,2)");

        builder.HasOne(o => o.Customer)
            .WithMany(c => c.Orders)
            .HasForeignKey(o => o.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(o => o.CustomerId)
            .HasDatabaseName("ix_orders_customer_id");

        builder.HasIndex(o => o.Status)
            .HasDatabaseName("ix_orders_status");
    }
}

// DbContext
public class AppDbContext : DbContext
{
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<Customer> Customers => Set<Customer>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
```

## Zero-Downtime Migration Strategy

For critical production changes, follow the expand-contract pattern:

```
Phase 1: EXPAND
  - Add new column/table (nullable or with default)
  - Deploy: app writes to BOTH old and new
  - Backfill existing data

Phase 2: MIGRATE
  - Deploy: app reads from NEW, writes to BOTH
  - Verify data consistency

Phase 3: CONTRACT
  - Deploy: app only uses NEW
  - Drop old column/table in separate migration
```

### Timeline Example

```
Day 1: Migration adds new_status column (nullable)
Day 1: Deploy app v2 — writes to both status and new_status
Day 2: Run backfill migration for existing rows
Day 3: Deploy app v3 — reads from new_status only
Day 7: Migration drops old status column
```

## Multi-Database Support

### PostgreSQL and SQLite (for testing)

```csharp
public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("orders");

        // PostgreSQL-specific
        if (builder.Metadata.ProviderName == "Npgsql.EntityFrameworkCore.PostgreSQL")
        {
            builder.Property(o => o.Id)
                .HasColumnType("uuid");
        }
        // SQLite-specific
        else if (builder.Metadata.ProviderName == "Microsoft.EntityFrameworkCore.Sqlite")
        {
            builder.Property(o => o.Id)
                .HasColumnType("TEXT");
        }
    }
}
```

## Rollback Strategy

```csharp
// When a migration fails, don't rollback the migration file
// Instead, create a new migration to fix the issue

// Example: If you accidentally dropped a column
protected override void Up(MigrationBuilder migrationBuilder)
{
    // Oops! This was applied accidentally
    migrationBuilder.DropColumn(name: "important_column", table: "users");
}

// Create a fix migration
protected override void Up(MigrationBuilder migrationBuilder)
{
    migrationBuilder.AddColumn<string>(
        name: "important_column",
        table: "users",
        type: "text",
        nullable: true);
}
```

## Anti-Patterns

| Anti-Pattern | Why It Fails | Better Approach |
|-------------|-------------|-----------------|
| Manual SQL in production | No audit trail, unrepeatable | Always use EF Core migrations |
| Editing deployed migrations | Causes drift between environments | Create new migration instead |
| NOT NULL without default | Locks table, rewrites all rows | Add nullable, backfill, then add constraint |
| Inline index on large table | Blocks writes during build | Use CONCURRENTLY or separate migration |
| Schema + data in one migration | Hard to rollback, long transactions | Separate migrations |
| Dropping column before removing code | Application errors | Remove code first, drop column next deploy |
