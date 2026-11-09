# سفارش و موجودی

## الگوهای استفاده‌شده

- **Repository:** دسترسی به داده را از handlerها جدا می‌کند و از پیاده‌سازی EF Core موجود در پروژه استفاده می‌کند.
- **CQRS و MediatR:** مسیر command و query و dispatch درخواست‌ها در پروژه برقرار است؛ برای همین عملیات سفارش نیز از همین ساختار پیروی می‌کند.
- **Unit of Work جداگانه نداریم:** مخزن‌ها از `DbContext` مشترک و scoped استفاده می‌کنند و `SaveChanges` مرز ذخیره اتمیک است. افزودن لایه دیگری در این سناریو ارزش کافی ندارد.

## ثبت امن سفارش

دو خرید هم‌زمان می‌توانند موجودی یکسانی بخوانند و بیش از موجودی بفروشند. ثبت سفارش و کسر موجودی در یک `SaveChanges` انجام می‌شود تا هر دو با هم commit یا rollback شوند. فیلد `Product.Version` کنترل هم‌زمانی است؛ درخواست بازنده `409` می‌گیرد. موجودی هر محصول در سفارش نیز پیش از ذخیره بررسی و تجمیع می‌شود.

## سفارش‌های پرتعداد مشتری

### عوامل کندی

اسکن جدول، `Skip` در صفحه‌های عمیق، محاسبه‌ی `COUNT(*)` کل در هر درخواست، بارگذاری entityهای کامل و query جداگانه برای هر سفارش (N+1) هزینه‌سازند.

### تغییرات Query و Application

`GET /api/Customers/{customerId}/Orders` اکنون از keyset pagination استفاده می‌کند. ترتیب پایدار `(CreatedAt DESC, Id DESC)` است؛ درخواست بعدی `afterCreatedAt` و `afterId` را از `nextCursor` می‌فرستد. `PageSize` پیش‌فرض ۲۰ و سقف آن ۱۰۰ است. `page` فقط برای صفحه‌ی اول پذیرفته می‌شود؛ صفحات بعد باید از cursor استفاده کنند.

Query فقط ستون‌های پاسخ را با `AsNoTracking` می‌خواند، `pageSize + 1` ردیف می‌گیرد تا `hasMore` را تعیین کند و دیگر total count اجرا نمی‌کند. تعداد اقلام به شکل projection در همان query محاسبه می‌شود، نه با query جدا برای هر سفارش؛ در نتیجه یک رفت‌وبرگشت به DB داریم.
پاسخ شامل `list`، `hasMore` و `nextCursor` است؛ مصرف‌کننده برای ادامه نباید به total count یا شماره صفحه متکی باشد.

```csharp
Where(customerId && (CreatedAt < cursorDate ||
    (CreatedAt == cursorDate && Id.CompareTo(cursorId) < 0)))
OrderByDescending(CreatedAt).ThenByDescending(Id)
Select(responseFields).Take(pageSize + 1)
```

### Indexها

```sql
CREATE INDEX IX_Orders_CustomerId_CreatedAt
ON Orders (CustomerId, RemovedAt, CreatedAt DESC, Id DESC)
INCLUDE (OrderNumber, OrderDate, TotalAmount, Status);

CREATE INDEX IX_OrderItems_OrderId_RemovedAt
ON OrderItems (OrderId, RemovedAt);
```

ایندکس Orders فیلتر مشتری و soft-delete، ترتیب و cursor را پوشش می‌دهد؛ ستون‌های `INCLUDE` lookup برای فیلدهای پاسخ را کم می‌کنند. ایندکس OrderItems، شمارش اقلام فعال هر سفارش را پشتیبانی می‌کند. هر دو در migration `OptimizeCustomerOrdersQuery` هستند.

اعمال migration:

```powershell
dotnet ef database update --project .\VisitorApp.Persistence --startup-project .\VisitorApp.Api
```

ساخت یا بازسازی ایندکس روی جداول بزرگ می‌تواند lock و زمان استقرار ایجاد کند؛ migration را ابتدا روی نسخه‌ی هم‌اندازه‌ی دیتابیس production بسنجید و برای production زمان‌بندی یا online index build متناسب با edition SQL Server در نظر بگیرید.

### سنجش و رشد آینده

تست‌ها عبور cursor، نبود هم‌پوشانی صفحات و مسیر API را بررسی می‌کنند؛ این‌ها benchmark دیتابیس نیستند. سنجش واقعی باید روی SQL Server با داده‌ی هم‌اندازه‌ی تولید، Actual Execution Plan، `SET STATISTICS IO, TIME ON` و p95/p99 latency انجام و با نسخه‌ی قبلی مقایسه شود. در این محیط benchmark SQL Server اجرا نشده است.

با رشد بیشتر، شمارش اقلام، مشتریان پرترافیک، حجم ایندکس و هزینه‌ی نوشتن می‌توانند گلوگاه شوند. پایش logical reads، حجم و fragmentation ایندکس‌ها ضروری است؛ در صورت نیاز cache، آرشیو/partitioning یا جداسازی read workload را با داده‌ی واقعی ارزیابی کنید.

## پاسخ خطا

Middleware پاسخ خطا را با `statusCode`، `traceId`، `data: null` و شیء `error` شامل پیام، جزئیات و خطاهای اعتبارسنجی برمی‌گرداند. خطای اعتبارسنجی `400`، نبود سفارش `404`، تعارض هم‌زمانی `409` و خطای پیش‌بینی‌نشده `500` است؛ جزئیات خطای `500` در Production افشا نمی‌شود.

## تغییر وضعیت

گذارهای مجاز: `Pending` به `Confirmed` یا `Cancelled`؛ `Confirmed` به `Completed` یا `Cancelled`. وضعیت‌های نهایی قابل تغییر نیستند.

## تست‌ها

اجرا:

```powershell
dotnet test --project .\Tests\VisitorApp.Application.Tests\VisitorApp.Application.Tests.csproj
```

تست‌های xUnit v3 با Microsoft Testing Platform، منطق سفارش و APIهای ایجاد، دریافت، فهرست مشتری و تغییر وضعیت را پوشش می‌دهند. APIها با `TestServer` و تست‌های handler با EF Core InMemory اجرا می‌شوند؛ هر تست نام دیتابیس مستقل می‌گیرد و به دیتابیس اصلی وصل نمی‌شود. یک تست جداگانه ترجمه‌ی cursor به SQL Server را بدون اتصال بررسی می‌کند. این روش‌ها benchmark واقعی، اجرای migration یا رقابت تراکنشی SQL Server نیستند؛ برای آن موارد باید از SQL Server تستیِ جداگانه استفاده شود.
