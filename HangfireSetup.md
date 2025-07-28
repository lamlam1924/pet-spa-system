# Hướng dẫn tích hợp Hangfire tự động gửi mail nhắc lịch

## 1. Cài đặt Hangfire

Mở terminal tại thư mục dự án và chạy:

```
dotnet add package Hangfire
```

## 2. Cấu hình Hangfire trong Program.cs hoặc Startup.cs

```csharp
using Hangfire;
using Hangfire.MemoryStorage; // Nếu muốn lưu job trong RAM, dùng cho demo/dev

// Trong phương thức ConfigureServices:
services.AddHangfire(x => x.UseMemoryStorage());
services.AddHangfireServer();

// Trong phương thức Configure (hoặc sau builder.Build() nếu dùng .NET 6+):
app.UseHangfireDashboard();
```

## 3. Đăng ký job gửi mail nhắc lịch mỗi ngày

Ví dụ trong Program.cs hoặc Startup.cs:

```csharp
// Đăng ký job chạy mỗi ngày lúc 8h sáng
RecurringJob.AddOrUpdate<IAppointmentService>(
    "send-appointment-reminders",
    service => service.SendUpcomingAppointmentReminders(),
    "0 8 * * *" // cron: 8h sáng mỗi ngày
);
```

## 4. Truy cập dashboard Hangfire

Mở trình duyệt: `http://localhost:5000/hangfire` (hoặc port bạn đang chạy)

## 5. Lưu ý
- Nếu deploy production, nên dùng storage thật (SQL Server, Redis...) thay vì MemoryStorage.
- Đảm bảo cấu hình SMTP đúng để gửi mail thành công.

---

Nếu cần tự động chèn code vào Program.cs/Startup.cs, hãy báo để mình thực hiện.
