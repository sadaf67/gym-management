# Tahila — سامانهٔ مدیریت باشگاه ورزشی

وب‌سایت و پنل مدیریت باشگاه بدن‌سازی و تناسب اندام: معرفی مربیان و کلاس‌ها، فروش اشتراک آنلاین، برنامهٔ تمرینی و تغذیه، و مدیریت کامل محتوای سایت از پنل ادمین.

## معماری

```
Tahila.Domain          # موجودیت‌ها و قواعد کسب‌وکار
Tahila.Application     # سرویس‌ها و منطق کاربرد
Tahila.Infrastructure  # EF Core، Repositoryها، درگاه پرداخت
Tahila.Web             # ASP.NET Core MVC + Area مدیریت
```

## تکنولوژی‌ها

ASP.NET Core 8 MVC · EF Core · SQL Server · Clean Architecture · Areas · Razor · Bootstrap RTL · درگاه زرین‌پال

## مدل دامنه

| موجودیت | نقش |
| --- | --- |
| `Coach` | پروفایل مربیان، تخصص و سوابق |
| `GymClass` / `OfflineClass` / `Schedule` | کلاس‌های آنلاین و حضوری و برنامهٔ هفتگی |
| `Plan` / `Subscription` / `Payment` | پلن‌های اشتراک، خرید و پرداخت آنلاین |
| `BodyAnalysis` | ثبت و پیگیری آنالیز ترکیب بدنی اعضا |
| `Transformation` | گالری قبل/بعد اعضا |
| `WorkoutArticle` / `NutritionArticle` | محتوای آموزشی تمرین و تغذیه |
| `Slider` / `GalleryItem` / `SiteSetting` | مدیریت کامل ظاهر و محتوای سایت از پنل |

## قابلیت‌ها

- **فروش اشتراک آنلاین** با اتصال به درگاه زرین‌پال (حالت sandbox برای توسعه) و فعال‌سازی خودکار اشتراک پس از تأیید پرداخت.
- **مدیریت کلاس‌ها و برنامهٔ هفتگی** با ظرفیت و زمان‌بندی.
- **پروفایل عضو**: اشتراک فعال، تاریخچهٔ پرداخت، آنالیز بدنی و روند تغییرات.
- **محتوای آموزشی** تمرین و تغذیه با دسته‌بندی.
- **پنل مدیریت کامل** (Area مجزا) برای مربیان، کلاس‌ها، پلن‌ها، مقالات، اسلایدر، گالری و تنظیمات سایت — بدون نیاز به تغییر کد.
- **دادهٔ اولیه** برای راه‌اندازی سریع در فایل‌های `seed_*.sql`.

## اجرای محلی

```bash
dotnet restore
dotnet ef database update --project Tahila.Infrastructure --startup-project Tahila.Web
dotnet run --project Tahila.Web
```

سپس در صورت نیاز، دادهٔ نمونه را اجرا کنید:

```bash
sqlcmd -S "(localdb)\mssqllocaldb" -d TahilaDb -i seed_workout.sql
sqlcmd -S "(localdb)\mssqllocaldb" -d TahilaDb -i seed_nutrition.sql
sqlcmd -S "(localdb)\mssqllocaldb" -d TahilaDb -i seed_sessions.sql
sqlcmd -S "(localdb)\mssqllocaldb" -d TahilaDb -i seed_transformations.sql
```

> `MerchantId` درگاه پرداخت و رشتهٔ اتصال واقعی را در `appsettings.Development.json` (خارج از گیت) یا `dotnet user-secrets` قرار دهید.
