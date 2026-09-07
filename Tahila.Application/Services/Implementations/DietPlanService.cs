using Tahila.Application.Models;
using Tahila.Application.Services.Interfaces;
using Tahila.Domain.Entities;

namespace Tahila.Application.Services.Implementations;

// ─────────────────────────────────────────────────────────────────────────────
// این سرویس برنامه غذایی شخصی‌سازی‌شده رو میسازه
// ورودیش: نتیجه BMI کاربر + اینکه VIP هست یا نه
// خروجیش: یه برنامه کامل شامل کالری، ماکروها، غذاها و (برای VIP) برنامه هفتگی
// ─────────────────────────────────────────────────────────────────────────────
public class DietPlanService : IDietPlanService
{
    // تابع اصلی — همه چیز از اینجا شروع میشه
    public DietPlanModel Generate(BodyAnalysis bmi, bool isVip)
    {
        // ════════════════════════════════════════════════════════════════
        //   مرحله ۱: تعیین هدف و کالری هدف
        // ════════════════════════════════════════════════════════════════

        string goal;
        int targetCal;

        if (bmi.BMI < 18.5f)
        {
            // کمبود وزن → باید وزن اضافه کنه → ۴۰۰ کالری بیشتر از نیاز روزانه
            goal      = "افزایش وزن";
            targetCal = (int)(bmi.TDEE + 400);
        }
        else if (bmi.BMI < 25f)
        {
            // وزن نرمال → هدف: حفظ وزن و عضله‌سازی → دقیقاً به اندازه نیاز
            goal      = "حفظ وزن و عضله‌سازی";
            targetCal = (int)bmi.TDEE;
        }
        else
        {
            // اضافه وزن → باید کم کنه → ۵۰۰ کالری کمتر از نیاز روزانه
            // ۵۰۰ کالری در روز = حدود ۰.۵ کیلو کاهش وزن در هفته
            goal      = "کاهش وزن";
            targetCal = (int)(bmi.TDEE - 500);
        }

        // حداقل کالری مجاز — زیر این نرو (برای سلامتی خطرناکه)
        // مرد: ۱۵۰۰ کالری، زن: ۱۲۰۰ کالری
        targetCal = Math.Max(targetCal, bmi.IsMale ? 1500 : 1200);

        // ════════════════════════════════════════════════════════════════
        //   مرحله ۲: محاسبه ماکروها (پروتئین، چربی، کربوهیدرات)
        // ════════════════════════════════════════════════════════════════

        // پروتئین: ۱.۸ تا ۲ گرم به ازای هر کیلو وزن
        // کسایی که اضافه وزن دارن، ۲ گرم (چون بیشتر باید عضله حفظ بشه)
        int protein = (int)(bmi.Weight * (bmi.BMI > 25 ? 2.0f : 1.8f));

        // چربی: ۲۷٪ از کل کالری — هر گرم چربی = ۹ کالری
        int fat = (int)(targetCal * 0.27f / 9);

        // کربوهیدرات: بقیه کالری — هر گرم کربو = ۴ کالری
        // ابتدا کالری پروتئین و چربی رو کم میکنیم، بقیه رو به کربو میدیم
        int carb = (int)((targetCal - protein * 4 - fat * 9) / 4);
        carb = Math.Max(carb, 50); // حداقل ۵۰ گرم کربو — کمتر از این مضره

        // ════════════════════════════════════════════════════════════════
        //   مرحله ۳: ساخت مدل برنامه
        // ════════════════════════════════════════════════════════════════

        var plan = new DietPlanModel
        {
            IsVip             = isVip,
            Weight            = bmi.Weight,
            Height            = bmi.Height,
            Age               = bmi.Age,
            IsMale            = bmi.IsMale,
            BMI               = bmi.BMI,
            BMICategory       = bmi.BMICategory,
            TDEE              = bmi.TDEE,
            BMR               = bmi.BMR,
            BodyFatPercentage = bmi.BodyFatPercentage,
            Goal              = goal,
            TargetCalories    = targetCal,
            ProteinGrams      = protein,
            CarbGrams         = Math.Max(carb, 0), // نباید منفی بشه
            FatGrams          = fat,
            WaterLiters       = bmi.IsMale ? 3 : 2, // مرد ۳ لیتر، زن ۲ لیتر آب
        };

        // نکات عمومی و دسته‌بندی غذایی — هم برای رایگان هم VIP
        plan.GeneralTips    = BuildTips(bmi, goal);
        plan.FoodCategories = BuildFoodCategories(bmi, goal);

        // برنامه هفتگی و لیست خرید و مکمل‌ها — فقط برای VIP
        if (isVip)
        {
            plan.WeeklyPlan   = BuildWeeklyPlan(targetCal, protein, bmi.IsMale, goal);
            plan.ShoppingList = BuildShoppingList(goal);
            plan.Supplements  = BuildSupplements(bmi, goal);
        }

        return plan;
    }

    // ════════════════════════════════════════════════════════════════
    //   نکات تغذیه‌ای (Tips)
    // ════════════════════════════════════════════════════════════════

    // لیستی از توصیه‌های مفید بر اساس هدف کاربر
    private static List<string> BuildTips(BodyAnalysis bmi, string goal)
    {
        // نکات مشترک برای همه
        var tips = new List<string>
        {
            $"هدف کالری روزانه شما {(int)(bmi.TDEE + (goal == "افزایش وزن" ? 400 : goal == "کاهش وزن" ? -500 : 0)):N0} کیلوکالری است.",
            "وعده‌های غذایی را به ۴ تا ۵ وعده کوچک در روز تقسیم کنید.",
            "هر وعده باید شامل پروتئین کافی برای حفظ عضلات باشد.",
            "حداقل ۳۰ دقیقه قبل و بعد از تمرین غذا بخورید.",
            $"روزانه حداقل {(bmi.IsMale ? 3 : 2)} لیتر آب بنوشید.",
            "از مصرف قند ساده و غذاهای فراوری‌شده تا حد امکان بپرهیزید.",
            "شب‌ها وعده سبک‌تری مثل پروتئین + سبزیجات بخورید.",
        };

        // نکات اضافه بر اساس هدف
        if (goal == "کاهش وزن")
        {
            tips.Add("کربوهیدرات‌های پیچیده مثل برنج قهوه‌ای و نان سبوس‌دار را جایگزین کنید.");
            tips.Add("قبل از غذا یک لیوان آب بنوشید تا احساس سیری سریع‌تر ایجاد شود.");
        }
        else if (goal == "افزایش وزن")
        {
            tips.Add("هر ۲-۳ ساعت یک وعده کوچک و مغذی مصرف کنید.");
            tips.Add("از آجیل، آووکادو و روغن زیتون برای افزایش کالری سالم استفاده کنید.");
        }

        return tips;
    }

    // ════════════════════════════════════════════════════════════════
    //   دسته‌بندی مواد غذایی مجاز
    // ════════════════════════════════════════════════════════════════

    // ۴ دسته اصلی: پروتئین، سبزیجات، کربوهیدرات، چربی‌های سالم
    private static List<FoodCategory> BuildFoodCategories(BodyAnalysis bmi, string goal)
    {
        var cats = new List<FoodCategory>
        {
            new()
            {
                Icon  = "🥩",
                Title = "پروتئین‌ها",
                Color = "#ef4444",
                // لیست غذا بر اساس جنسیت فرق میکنه
                Items = bmi.IsMale
                    ? new() { "سینه مرغ کبابی", "ماهی تن", "تخم‌مرغ", "گوشت قرمز کم‌چرب", "پنیر کم‌چرب", "لبنیات کم‌چرب" }
                    : new() { "سینه مرغ", "ماهی سالمون", "تخم‌مرغ", "لوبیا و عدس", "ماست یونانی", "پنیر کوتاژ" }
            },
            new()
            {
                Icon  = "🥦",
                Title = "سبزیجات",
                Color = "#22c55e",
                Items = new() { "کلم بروکلی", "اسفناج", "خیار و گوجه", "فلفل دلمه", "کاهو", "هویج" }
            },
            new()
            {
                Icon  = "🌾",
                Title = "کربوهیدرات‌های مجاز",
                Color = "#f97316",
                // کسایی که کم کنن، کربوهیدرات محدودتری دارن
                Items = goal == "کاهش وزن"
                    ? new() { "سیب‌زمینی شیرین", "برنج قهوه‌ای", "نان سبوس‌دار", "جو دوسر", "کینوا" }
                    : new() { "برنج ایرانی", "ماکارونی سبوس‌دار", "سیب‌زمینی", "نان سنگک", "جو دوسر", "کینوا" }
            },
            new()
            {
                Icon  = "🥑",
                Title = "چربی‌های سالم",
                Color = "#8b5cf6",
                Items = new() { "آووکادو", "روغن زیتون", "مغز گردو", "بادام", "دانه کتان", "روغن نارگیل" }
            },
        };

        return cats;
    }

    // ════════════════════════════════════════════════════════════════
    //   برنامه غذایی هفتگی — فقط برای کاربران VIP
    // ════════════════════════════════════════════════════════════════

    // برای هر روز هفته: صبحانه، ناهار، شام، میان‌وعده
    private static List<DayMealPlan> BuildWeeklyPlan(int kcal, int protein, bool isMale, string goal)
    {
        // توزیع کالری بین وعده‌ها:
        // صبحانه: ۲۵٪ | ناهار: ۳۵٪ | شام: ۲۵٪ | میان‌وعده: ۱۵٪
        int bfCal = (int)(kcal * .25);                             // صبحانه
        int luCal = (int)(kcal * .35);                             // ناهار
        int diCal = (int)(kcal * .25);                             // شام
        int snCal = kcal - bfCal - luCal - diCal;                  // میان‌وعده (باقیمانده)

        // اسامی ۷ روز هفته — فارسی و انگلیسی
        var days = new[]
        {
            ("شنبه","SAT"),("یکشنبه","SUN"),("دوشنبه","MON"),
            ("سه‌شنبه","TUE"),("چهارشنبه","WED"),("پنجشنبه","THU"),("جمعه","FRI")
        };

        // ۷ قالب مختلف برای هر نوع وعده — هر روز یکی استفاده میشه
        var breakfasts = new[]
        {
            new MealItem { Name="جو دوسر با شیر",       Description="۸۰گ جو دوسر + ۲۰۰مل شیر کم‌چرب + موز",          Calories=bfCal, Protein="۱۵گ", Icon="🥣" },
            new MealItem { Name="نیمرو و نان سبوس‌دار",  Description="۳ تخم‌مرغ + ۲ تکه نان سبوس‌دار + گوجه",         Calories=bfCal, Protein="۲۰گ", Icon="🍳" },
            new MealItem { Name="ماست یونانی با گرانولا", Description="۲۰۰گ ماست یونانی + ۴۰گ گرانولا + توت‌فرنگی",  Calories=bfCal, Protein="۱۸گ", Icon="🥛" },
            new MealItem { Name="اسموتی پروتئینی",        Description="موز + شیر + پودر پروتئین + کره بادام‌زمینی",   Calories=bfCal, Protein="۲۵گ", Icon="🥤" },
            new MealItem { Name="تخم‌مرغ آب‌پز و پنیر",   Description="۳ تخم‌مرغ آب‌پز + ۵۰گ پنیر کم‌چرب + خیار",  Calories=bfCal, Protein="۲۲گ", Icon="🥚" },
            new MealItem { Name="پن‌کیک پروتئینی",         Description="آرد جو + تخم‌مرغ + شیر + عسل",                Calories=bfCal, Protein="۱۸گ", Icon="🥞" },
            new MealItem { Name="آمله‌ت سبزیجات",          Description="۳ تخم‌مرغ + فلفل + قارچ + اسفناج",            Calories=bfCal, Protein="۲۱گ", Icon="🍳" },
        };

        var lunches = new[]
        {
            new MealItem { Name="سینه مرغ کبابی با برنج",   Description="۱۵۰گ مرغ + ۱ پیاله برنج قهوه‌ای + سالاد",    Calories=luCal, Protein="۴۰گ", Icon="🍗" },
            new MealItem { Name="ماهی سالمون با سیب‌زمینی", Description="۱۸۰گ سالمون + ۱۵۰گ سیب‌زمینی + کلم بروکلی", Calories=luCal, Protein="۴۵گ", Icon="🐟" },
            new MealItem { Name="خوراک گوشت و سبزیجات",     Description="۱۵۰گ گوشت کم‌چرب + لوبیا + هویج",            Calories=luCal, Protein="۳۸گ", Icon="🥩" },
            new MealItem { Name="پاستا مرغ",                 Description="۱۵۰گ پاستا سبوس‌دار + ۱۲۰گ مرغ + سس گوجه", Calories=luCal, Protein="۳۵گ", Icon="🍝" },
            new MealItem { Name="برگر مرغ خانگی",            Description="۱۵۰گ مرغ چرخ‌کرده + نان سبوس‌دار + سالاد",  Calories=luCal, Protein="۴۲گ", Icon="🍔" },
            new MealItem { Name="خوراک کوینوا و مرغ",        Description="۱۵۰گ کوینوا + ۱۳۰گ مرغ + سبزیجات بخار",    Calories=luCal, Protein="۴۰گ", Icon="🥗" },
            new MealItem { Name="ماهی تن با برنج",           Description="۱ قوطی ماهی تن + ۱ پیاله برنج + خیار",       Calories=luCal, Protein="۳۸گ", Icon="🐟" },
        };

        var dinners = new[]
        {
            new MealItem { Name="مرغ بخارپز با سبزیجات", Description="۱۳۰گ مرغ + اسفناج + کلم + فلفل",              Calories=diCal, Protein="۳۵گ", Icon="🥦" },
            new MealItem { Name="سوپ پروتئینی",           Description="مرغ + لوبیا + سبزیجات + آب‌مرغ",              Calories=diCal, Protein="۳۰گ", Icon="🍲" },
            new MealItem { Name="سالاد نیسواز",           Description="ماهی تن + تخم‌مرغ + لوبیا سبز + سالاد",      Calories=diCal, Protein="۳۲گ", Icon="🥗" },
            new MealItem { Name="کباب مرغ با کاهو",       Description="۱۳۰گ کباب مرغ + کاهو + گوجه + زیتون",        Calories=diCal, Protein="۳۸گ", Icon="🫕" },
            new MealItem { Name="تخم‌مرغ و سبزیجات",      Description="۳ تخم‌مرغ آب‌پز + اسفناج تفت‌داده + قارچ",   Calories=diCal, Protein="۲۸گ", Icon="🥚" },
            new MealItem { Name="ماهی بخارپز",            Description="۱۵۰گ ماهی + لیمو + سیب‌زمینی شیرین + بروکلی", Calories=diCal, Protein="۳۸گ", Icon="🐟" },
            new MealItem { Name="استیک سینه مرغ",         Description="۱۳۰گ مرغ گریل + کینوا + سالاد سبز",           Calories=diCal, Protein="۳۶گ", Icon="🍗" },
        };

        var snacks = new[]
        {
            new MealItem { Name="آجیل مخلوط",          Description="۳۰گ گردو + بادام + پسته",                  Calories=snCal, Protein="۸گ",  Icon="🥜" },
            new MealItem { Name="ماست و میوه",          Description="۱۵۰گ ماست کم‌چرب + سیب یا موز",           Calories=snCal, Protein="۷گ",  Icon="🍎" },
            new MealItem { Name="شیک موز پروتئینی",     Description="موز + شیر + ۱ قاشق پودر پروتئین",         Calories=snCal, Protein="۲۰گ", Icon="🥤" },
            new MealItem { Name="نان تست و آووکادو",    Description="۱ تکه نان سبوس‌دار + ۵۰گ آووکادو",        Calories=snCal, Protein="۵گ",  Icon="🥑" },
            new MealItem { Name="پنیر و خیار",          Description="۵۰گ پنیر کم‌چرب + خیار تازه",             Calories=snCal, Protein="۱۰گ", Icon="🧀" },
            new MealItem { Name="خرما و بادام",         Description="۳ عدد خرما + ۱۵گ بادام",                  Calories=snCal, Protein="۴گ",  Icon="🌴" },
            new MealItem { Name="تخم‌مرغ آب‌پز",        Description="۲ تخم‌مرغ آب‌پز + کمی نمک و فلفل",       Calories=snCal, Protein="۱۲گ", Icon="🥚" },
        };

        // برای هر روز هفته یه DayMealPlan میسازیم
        var result = new List<DayMealPlan>();
        for (int i = 0; i < 7; i++)
        {
            result.Add(new DayMealPlan
            {
                DayName       = days[i].Item1,  // مثلاً "شنبه"
                DayEn         = days[i].Item2,  // مثلاً "SAT"
                Breakfast     = breakfasts[i],  // صبحانه این روز
                Lunch         = lunches[i],     // ناهار این روز
                Dinner        = dinners[i],     // شام این روز
                Snack         = snacks[i],      // میان‌وعده این روز
                TotalCalories = bfCal + luCal + diCal + snCal, // جمع کل
            });
        }

        return result;
    }

    // ════════════════════════════════════════════════════════════════
    //   لیست خرید هفتگی — فقط VIP
    // ════════════════════════════════════════════════════════════════

    private static List<string> BuildShoppingList(string goal)
    {
        // اقلام پایه که همه نیاز دارن
        var list = new List<string>
        {
            "سینه مرغ (۱ کیلو)", "ماهی سالمون یا ماهی تن (۴ قوطی/۵۰۰گ)",
            "تخم‌مرغ (۱۲ عدد)", "شیر کم‌چرب (۱ لیتر)",
            "ماست یونانی (۵۰۰گ)", "پنیر کم‌چرب (۲۰۰گ)",
            "اسفناج تازه (۳۰۰گ)", "کلم بروکلی (۵۰۰گ)",
            "خیار و گوجه", "فلفل دلمه (۳ رنگ)",
            "جو دوسر (۵۰۰گ)", "برنج قهوه‌ای (۵۰۰گ)",
            "نان سبوس‌دار (۱ بسته)", "کینوا (۳۰۰گ)",
            "روغن زیتون (۱ بطری)", "بادام و گردو (۲۰۰گ)",
            "لیمو ترش", "سیر و پیاز",
            "ادویه‌جات: زردچوبه، زیره، پاپریکا",
        };

        // اگه هدف افزایش وزنه، اقلام پرکالری اضافه میکنیم
        if (goal == "افزایش وزن")
        {
            list.Add("آووکادو (۳-۴ عدد)");
            list.Add("کره بادام‌زمینی (۱ شیشه)");
            list.Add("موز (۱ شانه)");
        }

        return list;
    }

    // ════════════════════════════════════════════════════════════════
    //   توصیه مکمل‌ها — فقط VIP
    // ════════════════════════════════════════════════════════════════

    private static List<string> BuildSupplements(BodyAnalysis bmi, string goal)
    {
        // مکمل‌های پایه برای همه
        var sups = new List<string>
        {
            "🥛 پودر پروتئین وی: ۱ سروینگ بعد از تمرین برای ریکاوری عضلات",
            "🐟 امگا-۳: ۲ کپسول در روز برای سلامت قلب و کاهش التهاب",
            "💊 ویتامین D3: ۲۰۰۰ IU روزانه (مخصوصاً در فصل زمستان)",
            "⚡ کراتین مونوهیدرات: ۵ گرم روزانه برای افزایش قدرت و توده عضلانی",
        };

        // مکمل ویژه کاهش وزن
        if (goal == "کاهش وزن")
            sups.Add("🍵 ال-کارنیتین: ۱۵۰۰ میلی‌گرم قبل از تمرین برای چربی‌سوزی بهتر");

        // بالای ۳۵ سال → کلسیم هم اضافه میکنیم
        if (bmi.Age > 35)
            sups.Add("🦴 کلسیم + ویتامین K2: برای سلامت استخوان‌ها");

        // همیشه این هشدار آخر باشه
        sups.Add("⚠️ توجه: قبل از مصرف هر مکملی با پزشک مشورت کنید.");

        return sups;
    }
}
