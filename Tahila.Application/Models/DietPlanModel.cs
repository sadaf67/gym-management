namespace Tahila.Application.Models;

public class DietPlanModel
{
    public bool IsVip { get; set; }

    // اطلاعات پایه از BMI
    public float Weight { get; set; }
    public float Height { get; set; }
    public int Age { get; set; }
    public bool IsMale { get; set; }
    public float BMI { get; set; }
    public string BMICategory { get; set; } = string.Empty;
    public float TDEE { get; set; }
    public float BMR { get; set; }
    public float? BodyFatPercentage { get; set; }

    // هدف و کالری
    public string Goal { get; set; } = string.Empty;         // "کاهش وزن" / "حفظ وزن" / "افزایش وزن"
    public int TargetCalories { get; set; }
    public int ProteinGrams { get; set; }
    public int CarbGrams { get; set; }
    public int FatGrams { get; set; }
    public int WaterLiters { get; set; }

    // رایگان: نکات کلی
    public List<string> GeneralTips { get; set; } = new();
    public List<FoodCategory> FoodCategories { get; set; } = new();

    // VIP: برنامه کامل هفتگی
    public List<DayMealPlan> WeeklyPlan { get; set; } = new();
    public List<string> ShoppingList { get; set; } = new();
    public List<string> Supplements { get; set; } = new();
}

public class FoodCategory
{
    public string Icon { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public List<string> Items { get; set; } = new();
}

public class DayMealPlan
{
    public string DayName { get; set; } = string.Empty;
    public string DayEn { get; set; } = string.Empty;
    public MealItem Breakfast { get; set; } = new();
    public MealItem Lunch { get; set; } = new();
    public MealItem Dinner { get; set; } = new();
    public MealItem Snack { get; set; } = new();
    public int TotalCalories { get; set; }
}

public class MealItem
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Calories { get; set; }
    public string Protein { get; set; } = string.Empty;
    public string Icon { get; set; } = "🍽️";
}
