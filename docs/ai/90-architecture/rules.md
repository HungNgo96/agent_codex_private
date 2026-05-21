# Architecture Rules

## General Rules

- Giữ cấu trúc hiện tại: UI trong `src/Components`, service/repository trong `src/Services`, model trong `src/Models`, EF config trong `src/Data/Configurations`.
- Không trộn business logic phức tạp trực tiếp vào Razor markup nếu có thể đặt trong service.
- Không để service phụ thuộc vào UI component.
- Không hardcode path, secret hoặc production config trong code.

## Dependency Injection Rules

- Service dùng trong UI phải được đăng ký trong `Program.cs`.
- Ưu tiên scoped service cho repository/service thao tác `AppDbContext`.
- Không tạo `AppDbContext` thủ công ngoài DI.
- Nếu cần scope mới trong background/batch operation, dùng `IServiceScopeFactory` như pattern hiện tại.

## EF Core Rules

- Schema thay đổi phải đi kèm migration khi cần.
- Configuration nên đặt trong class riêng dưới `src/Data/Configurations`.
- Index unique phải phản ánh invariant nghiệp vụ thật.
- DateTime được normalize UTC qua converter trong `AppDbContext`.
- Không tắt warning hoặc thay đổi convention nếu chưa hiểu impact.

## Service Rules

- Service phải validate input tại boundary.
- Database calls trong batch nên dùng lookup/batch query thay vì từng dòng.
- Exception message trả user phải rõ nhưng không lộ secret/stack trace.
- Log server-side phải có context đủ để debug.

## UI Rules

- Page/component nên gọi service async và hiển thị loading/error state.
- Form phải validate required fields trước khi save.
- File export/import phải có user feedback rõ.
- Shared component thay đổi phải kiểm tra các page đang dùng lại.

## Documentation Rules

- Module docs nằm dưới `docs/ai/<module>`.
- Mỗi module nên có `README.md`, `rules.md`, `decisions.md`.
- Plan/review/changelog chỉ tạo khi task có business logic, database, payroll/tax/email impact, security/performance risk, hoặc user yêu cầu.
- Không lưu copy source code trong `docs/ai`.
