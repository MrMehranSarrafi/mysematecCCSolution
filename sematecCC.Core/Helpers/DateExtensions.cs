using System.Globalization;
using System.Text.RegularExpressions;

namespace CardNoGenerator.Core;

public static class DateExtensions
{
    public static string ToPersian(this DateTime? dtMiladi)
    {
        if (dtMiladi == null)
            return "";

        PersianCalendar pc = new PersianCalendar();
        int year = pc.GetYear(dtMiladi.Value);
        int month = pc.GetMonth(dtMiladi.Value);
        int day = pc.GetDayOfMonth(dtMiladi.Value);

        return $"{year}/{month:00}/{day:00}";

    }
    public static string ToPersian(this DateTime dtMiladi)
    {
        PersianCalendar pc = new PersianCalendar();
        int year = pc.GetYear(dtMiladi);
        int month = pc.GetMonth(dtMiladi);
        int day = pc.GetDayOfMonth(dtMiladi);

        return $"{year}/{month:00}/{day:00}";

    }
    public static DateTime? ToMiladi(this string? shamsi)
    {
        // 1. اعتبارسنجی فرمت
        if (string.IsNullOrWhiteSpace(shamsi) || !Regex.IsMatch(shamsi, @"^\d{4}/\d{2}/\d{2}$"))
        {
            return null;
        }

        try
        {
            var parts = shamsi.Split('/');
            int year = int.Parse(parts[0]);
            int month = int.Parse(parts[1]);
            int day = int.Parse(parts[2]);

            // 2. ایجاد نمونه از تقویم شمسی
            PersianCalendar pc = new PersianCalendar();

            // 3. تبدیل به DateTime میلادی
            // ساعت، دقیقه، ثانیه و میلی‌ثانیه را صفر در نظر می‌گیریم
            return pc.ToDateTime(year, month, day, 0, 0, 0, 0);
        }
        catch (ArgumentOutOfRangeException)
        {
            // اگر تاریخ نامعتبر باشد (مثلاً ۳۱/۲/۱۴۰۰)
            return null;
        }

         
        //return new DateTime(year, month, day, pc);
         
    }

}
