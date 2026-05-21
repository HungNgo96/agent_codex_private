# Architecture

## Purpose

Folder này ghi lại bối cảnh kiến trúc chung của ứng dụng PayrollSolution để AI hiểu cấu trúc hệ thống trước khi sửa các module nghiệp vụ.

## Application Shape

- .NET Blazor application
- Entity Framework Core persistence
- Service/repository classes under `src/Services`
- Domain/data models under `src/Models`
- EF configurations under `src/Data/Configurations`
- UI pages/components under `src/Components`
- Static assets and import templates under `src/wwwroot`

## Main Files

- `src/Program.cs`
- `src/Data/AppDbContext.cs`
- `src/Data/Conventions/SnakeCaseNamingConvention.cs`
- `src/Data/Configurations/*.cs`
- `src/Services/*.cs`
- `src/Models/*.cs`
- `src/Components/Pages/*.razor`
- `src/Components/Shared/*.razor`

## Main Flows

### Request/UI Flow

1. User thao tác trên Blazor page hoặc shared component.
2. Component gọi service/repository đã đăng ký DI trong `Program.cs`.
3. Service validate nghiệp vụ và đọc/ghi qua `AppDbContext`.
4. EF configuration kiểm soát table, index, length và relationship.
5. UI hiển thị kết quả, validation hoặc file export.

### Data Flow

1. Models định nghĩa dữ liệu ứng dụng.
2. `AppDbContext` expose `DbSet`.
3. Configuration classes định nghĩa schema.
4. Migrations ghi lại thay đổi database.
5. Services thao tác dữ liệu bằng EF Core async APIs.

## Related Modules

- `01-employee-management`
- `02-payroll-calculation`
- `03-email-notification`
- `04-tax-report`
- `05-contract`

## AI Notes

Khi AI sửa kiến trúc:

- Đọc module docs liên quan trước khi sửa business logic.
- Không đổi schema hoặc migration nếu task không yêu cầu.
- Không đưa secret vào appsettings hoặc source code.
- Giữ service/repository pattern hiện tại trừ khi có refactor rõ.
- Nếu thay đổi DI registration, kiểm tra các page/service đang inject dependency đó.
- Nếu thay đổi `AppDbContext`, phải kiểm tra EF migration impact.
