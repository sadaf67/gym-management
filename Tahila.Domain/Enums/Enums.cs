namespace Tahila.Domain.Enums;

public enum Gender { Male, Female, Both }

public enum DayOfWeekPersian
{
    Saturday = 0,
    Sunday = 1,
    Monday = 2,
    Tuesday = 3,
    Wednesday = 4,
    Thursday = 5,
    Friday = 6
}

public enum SubscriptionStatus { Active, Expired, Pending, Cancelled }

public enum PaymentStatus { Pending, Success, Failed, Refunded }

public enum PaymentGateway { ZarinPal }
