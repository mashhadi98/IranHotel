namespace Shared.Helpers;

public static class DateTimeHelper
{
    public static PersianDayOfWeek GetPersianDayOfWeek(this DateTime date)
    {
        switch (date.DayOfWeek)
        {
            case DayOfWeek.Saturday:
                return PersianDayOfWeek.Saturday;
            case DayOfWeek.Sunday:
                return PersianDayOfWeek.Sunday;
            case DayOfWeek.Monday:
                return PersianDayOfWeek.Monday;
            case DayOfWeek.Tuesday:
                return PersianDayOfWeek.Tuesday;
            case DayOfWeek.Wednesday:
                return PersianDayOfWeek.Wednesday;
            case DayOfWeek.Thursday:
                return PersianDayOfWeek.Thursday;
            case DayOfWeek.Friday:
                return PersianDayOfWeek.Friday;
            default:
                throw new Exception();
        }
    }

    private static DateTime _d1970 = new(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

    public static DateTime ToUtcDateTime(this double val)
    {
        return _d1970.Add(TimeSpan.FromMicroseconds(val));
    }

    public static double ToUtcUnixTimeStamp(this DateTime dateTime)
    {
        return dateTime.ToUniversalTime().Subtract(_d1970).TotalNanoseconds;
    }
}

public enum PersianDayOfWeek
{
    Saturday,
    Sunday,
    Monday,
    Tuesday,
    Wednesday,
    Thursday,
    Friday
}