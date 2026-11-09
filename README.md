# سفارش و موجودی

## تنظیمات ذخیره‌شده در User Secrets

برای اجرای محلی در محیط Development، این تنظیمات در User Secrets پروژه‌ی API ذخیره شده‌اند و در فایل‌های مخزن قرار ندارند:

- اتصال دیتابیس: `ConnectionStrings:ApplicationDb`
- JWT: `Jwt:SigningKey`، `Jwt:Issuer`، `Jwt:Audience`، `Jwt:AccessTokenMinutes`، `Jwt:RefreshTokenDays` و `Jwt:RequireHttpsMetadata`
- مدیر اولیه: `AdminSeed:Email`، `AdminSeed:Password`، `AdminSeed:FirstName` و `AdminSeed:LastName`
- کلاینت OpenIddict: `OpenIddict:Client:Id`، `Secret`، `DisplayName`، `RedirectUris:0`، `RedirectUris:1` و `PostLogoutRedirectUris:0`

برای عوض‌کردن هر مقدار، از ریشه‌ی repository دستور `dotnet user-secrets set` را با نام کامل کلید اجرا کنید. برای مثال:

```powershell
dotnet user-secrets set "ConnectionStrings:ApplicationDb" "<new connection string>" --project .\VisitorApp.Api\VisitorApp.API.csproj
dotnet user-secrets set "Jwt:SigningKey" "<new random key of at least 32 bytes>" --project .\VisitorApp.Api\VisitorApp.API.csproj
dotnet user-secrets set "AdminSeed:Password" "<new initial admin password>" --project .\VisitorApp.Api\VisitorApp.API.csproj
dotnet user-secrets set "OpenIddict:Client:Secret" "<new client secret>" --project .\VisitorApp.Api\VisitorApp.API.csproj
```

همین دستور برای هر کلید فهرست‌شده قابل استفاده است. مقادیر را می‌توان با `dotnet user-secrets list --project .\VisitorApp.Api\VisitorApp.API.csproj` دید؛ از اجرای آن در خروجی‌های مشترک یا ضبط‌شده خودداری کنید، چون secretها را نمایش می‌دهد. پس از تغییر، برنامه را restart کنید. اگر کلیدی در این دستگاه تنظیم نشده باشد، دستور `set` آن را ایجاد می‌کند.

## تنظیمات محیطی

مقادیر وابسته به محیط و secretها در فایل‌های source نگهداری نمی‌شوند. برنامه هنگام شروع، نبود connection string یا JWT نامعتبر را با خطای واضح رد می‌کند. در PowerShell متغیرهای زیر را برای هر محیط تنظیم کنید:

```powershell
$env:ConnectionStrings__ApplicationDb = "<SQL Server connection string>"
$env:Jwt__SigningKey = "<base64-encoded random key of at least 32 bytes>"
$env:Jwt__Issuer = "<issuer>"
$env:Jwt__Audience = "<audience>"
$env:Jwt__AccessTokenMinutes = "60"
$env:Jwt__RefreshTokenDays = "30"
$env:Jwt__RequireHttpsMetadata = "true"
$env:AdminSeed__Email = "<initial admin email>"
$env:AdminSeed__Password = "<strong initial admin password>"
$env:AdminSeed__FirstName = "<first name>"
$env:AdminSeed__LastName = "<last name>"
$env:OpenIddict__Client__Id = "<client id>"
$env:OpenIddict__Client__Secret = "<client secret>"
$env:OpenIddict__Client__DisplayName = "<client display name>"
$env:OpenIddict__Client__RedirectUris__0 = "<absolute redirect URI>"
$env:OpenIddict__Client__PostLogoutRedirectUris__0 = "<absolute post-logout URI>"
```

برای ساخت کلید تصادفی در PowerShell از
`[Convert]::ToBase64String([Security.Cryptography.RandomNumberGenerator]::GetBytes(32))`
استفاده کنید. مقادیر واقعی را در secret manager محیط اجرا نگه دارید، نه در repository یا `launchSettings.json`.
EF Core design-time factory نیز فقط `ConnectionStrings__ApplicationDb` را می‌خواند؛ بنابراین migration و اجرای API روی یک دیتابیسِ از پیش تنظیم‌شده انجام می‌شوند:

```powershell
# تنظیم محلی برای Development (یک‌بار؛ مقدار واقعی را فقط روی سیستم خودتان وارد کنید)
dotnet user-secrets set "ConnectionStrings:ApplicationDb" "<SQL Server connection string>" --project .\VisitorApp.Api\VisitorApp.API.csproj

# یا در همان PowerShell که برنامه را اجرا می‌کنید:
$env:ConnectionStrings__ApplicationDb = "<SQL Server connection string>"
dotnet run --project .\VisitorApp.Api\VisitorApp.API.csproj --launch-profile http

# Migration در PowerShell:
$env:ConnectionStrings__ApplicationDb = "<SQL Server connection string>"
dotnet ef database update --project .\VisitorApp.Persistence --startup-project .\VisitorApp.Api
```

`dotnet user-secrets` در محیط Development توسط `WebApplication.CreateBuilder` بارگذاری می‌شود و برای پروفایل‌های `http`، `https` و IIS Express مشترک است. برای محیط‌های غیر Development از environment variable یا secret manager همان محیط استفاده کنید.

تنظیمات JWT، مدیر اولیه و OpenIddict نیز در Development می‌توانند با همان کلیدها در User Secrets ذخیره شوند؛ برای مثال:

```powershell
dotnet user-secrets set "Jwt:SigningKey" "<random key of at least 32 bytes>" --project .\VisitorApp.Api\VisitorApp.API.csproj
dotnet user-secrets set "Jwt:Issuer" "<issuer>" --project .\VisitorApp.Api\VisitorApp.API.csproj
dotnet user-secrets set "Jwt:Audience" "<audience>" --project .\VisitorApp.Api\VisitorApp.API.csproj
dotnet user-secrets set "Jwt:AccessTokenMinutes" "60" --project .\VisitorApp.Api\VisitorApp.API.csproj
dotnet user-secrets set "Jwt:RefreshTokenDays" "30" --project .\VisitorApp.Api\VisitorApp.API.csproj
dotnet user-secrets set "Jwt:RequireHttpsMetadata" "true" --project .\VisitorApp.Api\VisitorApp.API.csproj
```

تنظیمات `AdminSeed:*` و `OpenIddict:Client:*` نیز هنگام شروع اجباری‌اند؛ برای هرکدام از کلیدهای فهرست environment بالا، بخش `:`-جداشده را به `dotnet user-secrets set` بدهید. پس از تغییر secretها، برنامه را restart کنید.

فایل‌های migration شامل نام جدول‌ها، ستون‌ها و ایندکس‌های لازم برای بازسازی نسخه‌ی schema هستند و باید در source control باقی بمانند؛ آن‌ها نباید شامل آدرس دیتابیس، credential یا تنظیمات مختص یک محیط باشند.

## اجرای محلی با Docker Compose

پیش‌نیاز: Docker Desktop با پشتیبانی از Linux containers. از ریشه‌ی repository در PowerShell اجرا کنید:

```powershell
Copy-Item .env.example .env
```

مقادیر خالی در `.env` را تنظیم کنید:

- `MSSQL_SA_PASSWORD`: رمز قوی SQL Server، با حروف بزرگ و کوچک، عدد و نماد؛ در این مقدار از `;` استفاده نکنید.
- `JWT_SIGNING_KEY`: کلید تصادفی حداقل ۳۲ بایتی. می‌توانید در PowerShell آن را بسازید:
  `[Convert]::ToBase64String([Security.Cryptography.RandomNumberGenerator]::GetBytes(32))`
- `ADMIN_SEED_PASSWORD`: رمز مدیر اولیه؛ حداقل ۸ نویسه و شامل حرف بزرگ، حرف کوچک و عدد.
- `OPENIDDICT_CLIENT_SECRET`: secret کلاینت محلی.

سپس سرویس‌ها را بسازید و اجرا کنید:

```powershell
docker compose up --build
```

API در `http://localhost:8080` و مستندات Swagger در `http://localhost:8080/swagger` در دسترس است. API پس از سالم‌شدن SQL Server شروع می‌شود و migrationهای موجود را هنگام راه‌اندازی اعمال می‌کند. دیتابیس در volume با نام `sqlserver-data` نگهداری می‌شود؛ برای توقف سرویس‌ها بدون حذف داده‌ها از `docker compose down` استفاده کنید. اجرای `docker compose down -v` volume و داده‌های محلی دیتابیس را حذف می‌کند.

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

## Cache محصول

`GET /api/Products/{id}` از cache-aside حافظه‌ای با عمر ۳۰ ثانیه استفاده می‌کند: ابتدا cache بررسی می‌شود؛ در miss فقط فیلدهای لازم از دیتابیس خوانده و cache می‌شوند. بعد از commit موفق سفارش و کسر موجودی، ورودی محصول invalidate می‌شود تا درخواست بعدی قیمت/موجودی تازه را از دیتابیس بارگذاری کند. هر مسیر ویرایش یا حذف Product نیز باید پس از commit همین invalidation را اجرا کند.

Cache محلی است؛ در چند instance، invalidation به‌تنهایی فقط همان instance را پوشش می‌دهد و تا TTL ممکن است instance دیگر داده‌ی قدیمی بدهد. برای آن محیط از `IDistributedCache` همراه invalidation پیام‌محور استفاده کنید. در همین پیاده‌سازی خرابی/ری‌استارت برنامه cache را خالی می‌کند و درخواست‌ها به DB fallback می‌کنند؛ cache hit در زمان اختلال DB قابل پاسخ است، ولی miss بدون DB خطا می‌دهد. انقضای هم‌زمان کلیدهای پرترافیک می‌تواند cache stampede بسازد؛ TTL پراکنده، single-flight و پایش hit-rate/DB load راهکارهای بعدی‌اند.

Trade-off: cache فشار و latency خواندن را کم می‌کند، اما هزینه‌ی حافظه و احتمال داده‌ی کوتاه‌مدت stale دارد؛ برای عملیات حساس مثل اعتبارسنجی/کسر موجودی، cache استفاده نمی‌شود و DB مرجع نهایی است.

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

## مسیر تکامل سفارش به سرویس‌های مستقل

این بخش طرح مقصد است؛ کد فعلی هنوز monolith با یک دیتابیس است و Outbox، broker و سرویس‌های مستقل در آن پیاده‌سازی نشده‌اند. برای آماده‌سازی، ابتدا ماژول‌های `Orders`، `Inventory`، `Payments` و `Notifications` را در همین برنامه با مسئولیت و قرارداد داخلی مشخص کنید؛ جداسازی deployment را تا وقتی نیاز واقعی وجود ندارد به تعویق بیندازید.

### اجزا و جریان

- **Order/Saga:** سفارش، مرحله‌ی جاری و نتیجه‌ی هر مرحله را نگه می‌دارد و جریان را هماهنگ می‌کند.
- **Inventory:** رزرو و آزادسازی موجودی را انجام می‌دهد.
- **Payment:** دریافت یا برگشت وجه را انجام می‌دهد.
- **Notification:** نتیجه‌ی نهایی را برای مشتری ارسال می‌کند؛ خرابی اعلان نباید پرداخت یا سفارش را دوباره اجرا کند.
- **Broker:** commandها و eventها را پایدار بین سرویس‌ها منتقل می‌کند؛ سرویس‌ها فقط دیتابیس خود را می‌نویسند.

جریان نمونه: `OrderPending` ← ثبت سفارش؛ `ReserveInventory` → `InventoryReserved`؛ `CapturePayment` → `PaymentCaptured`؛ سپس سفارش `Confirmed` و `OrderConfirmed` برای اعلان منتشر می‌شود. رد رزرو یا پرداخت، فرایند را به شکست می‌برد؛ در صورت نیاز `ReleaseInventory` یا `RefundPayment` عملیات جبرانی است.

### پایداری، تکرار و خرابی

- تغییر state و پیام خروجی در **یک تراکنش محلی** در جدول Outbox ذخیره می‌شوند. worker پیام‌های ارسال‌نشده را پس از restart دوباره منتشر می‌کند.
- هر مصرف‌کننده، `MessageId` را در Inbox با قید یکتا ثبت می‌کند و ثبت پیام و تغییر محلی را اتمیک انجام می‌دهد. پیام تکراری ACK می‌شود، اما اثر تجاری دوباره اعمال نمی‌شود.
- API سفارش `Idempotency-Key` را ذخیره می‌کند؛ retry همان کلید، همان نتیجه/شناسه سفارش را برمی‌گرداند، نه سفارش جدید. کلیدهای تکراری با payload متفاوت رد می‌شوند.
- تحویل پیام **حداقل یک‌بار** است، نه exactly-once. مصرف‌کننده باید idempotent باشد؛ retry با backoff، صف خطا برای پیام‌های ناموفق دائمی و replay کنترل‌شده لازم است.
- وضعیت Saga، شناسه‌ی عملیات هر مرحله، retryها و خطاها در دیتابیس Order نگهداری می‌شوند؛ با restart از state ذخیره‌شده ادامه می‌یابد. timeout مرحله‌ی رزرو، آزادسازی رزرو را فعال می‌کند؛ اعلان شکست‌خورده جداگانه retry می‌شود.
- اگر DB یا broker در دسترس نباشد، درخواستِ بدون commit موفق اعلام نمی‌شود؛ پیام Outbox تا بازیابی ذخیره/ارسال می‌شود. پس از قطع ارتباط، مصرف‌کننده backlog را با deduplication پردازش می‌کند. توقف بعد از commit و پیش از publish نیز با Outbox پوشش داده می‌شود.

### ناسازگاری و هزینه‌ها

پیام دیررس یا خارج از ترتیب، timeout هم‌زمان با پاسخ موفق، retry پس از پاسخ نامعلوم و شکست عملیات جبرانی می‌توانند state را ناسازگار کنند. هر پیام باید `SagaId` و شناسه‌ی مرحله داشته باشد؛ transitionها شرطی و versioned باشند، پاسخ مرحله‌ی قدیمی نادیده گرفته شود و Sagaهای معطل برای بررسی/جبران پایش شوند. رزرو موجودی باید TTL داشته باشد تا رزرو رهاشده دائمی نشود.

نتیجه، **سازگاری نهایی** است: سفارش ممکن است موقتاً `Pending` بماند. Outbox/Inbox، broker، پایش و replay پیچیدگی عملیاتی دارند و Saga جای تراکنش توزیع‌شده را با منطق جبرانی می‌گیرد. تا قبل از جداسازی واقعی، تراکنش محلی فعلی سفارش و موجودی را حفظ کنید؛ هنگام مهاجرت، ابتدا Outbox و idempotency را در monolith اضافه و تست کنید، سپس ماژول‌ها را یکی‌یکی به سرویس و دیتابیس مستقل منتقل کنید.
