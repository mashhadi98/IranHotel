# سفارش و موجودی

## الگوهای استفاده‌شده

- **Repository:** دسترسی به داده را از handlerها جدا می‌کند و از پیاده‌سازی EF Core موجود در پروژه استفاده می‌کند.
- **CQRS و MediatR:** مسیر command و query و dispatch درخواست‌ها در پروژه برقرار است؛ برای همین عملیات سفارش نیز از همین ساختار پیروی می‌کند.
- **Unit of Work جداگانه نداریم:** مخزن‌ها از `DbContext` مشترک و scoped استفاده می‌کنند و `SaveChanges` مرز ذخیره اتمیک است. افزودن لایه دیگری در این سناریو ارزش کافی ندارد.

## ثبت امن سفارش

دو خرید هم‌زمان می‌توانند موجودی یکسانی بخوانند و بیش از موجودی بفروشند. ثبت سفارش و کسر موجودی در یک `SaveChanges` انجام می‌شود تا هر دو با هم commit یا rollback شوند. فیلد `Product.Version` کنترل هم‌زمانی است؛ درخواست بازنده `409` می‌گیرد. موجودی هر محصول در سفارش نیز پیش از ذخیره بررسی و تجمیع می‌شود.

## سفارش‌های پرتعداد مشتری

برای جدول `Orders` با بیش از ۲۰ میلیون رکورد، درخواست پرتکرار باید مشتری را فیلتر و بر اساس تاریخ ایجاد مرتب و صفحه‌بندی کند:

```csharp
db.Orders
    .Where(o => o.CustomerId == customerId)
    .OrderByDescending(o => o.CreatedAt)
    .Skip(offset)
    .Take(pageSize);
```

درخواست `GetCustomerOrders` همین فیلتر را اعمال می‌کند و صفحه‌بندی فعلی مخزن، مرتب‌سازی نزولی `CreatedAt` دارد.

### تغییر پایگاه داده و Index

Migration، ایندکس مرکب زیر را به `Orders` اضافه می‌کند:

```sql
CREATE INDEX IX_Orders_CustomerId_CreatedAt
ON Orders (CustomerId ASC, RemovedAt ASC, CreatedAt DESC)
INCLUDE (OrderNumber, OrderDate, TotalAmount, Status);
```

`CustomerId` و `RemovedAt` فیلتر مشتری و soft-delete را اعمال می‌کنند؛ `CreatedAt DESC` ترتیب را پوشش می‌دهد و ستون‌های `INCLUDE` خواندن اطلاعات خروجی را ارزان‌تر می‌کنند. ایندکس مرکب همچنین جای ایندکس تک‌ستونی `CustomerId` را می‌گیرد.

Migration در `VisitorApp.Persistence/Migrations` قرار دارد و با دستور زیر اعمال می‌شود:

```powershell
dotnet ef database update --project .\VisitorApp.Persistence --startup-project .\VisitorApp.Api
```

### رشد بیشتر داده

`Skip/Take` صفحه‌های عمیق را پرهزینه می‌کند؛ برای آن‌ها از keyset pagination بر پایه‌ی `(CreatedAt, Id)` استفاده کنید. ایندکس‌ها هزینه‌ی نوشتن و دیسک دارند؛ کارایی را با execution plan و بار واقعی پایش کنید. در مقیاس بالاتر، آرشیو یا partitioning بر اساس تاریخ را ارزیابی کنید.

## پاسخ خطا

Middleware پاسخ خطا را با `statusCode`، `traceId`، `data: null` و شیء `error` شامل پیام، جزئیات و خطاهای اعتبارسنجی برمی‌گرداند. خطای اعتبارسنجی `400`، نبود سفارش `404`، تعارض هم‌زمانی `409` و خطای پیش‌بینی‌نشده `500` است؛ جزئیات خطای `500` در Production افشا نمی‌شود.
