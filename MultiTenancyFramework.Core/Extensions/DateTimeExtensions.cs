using System;

namespace MultiTenancyFramework
{
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Pass in the GMT difference. Defaults to +1, whith is Nigerian time
        /// </summary>
        /// <param name="date"></param>
        /// <param name="hourDifference">The -ve or +ve difference between GMT and your location. Eg. Nogeria is GMT+1</param>
        /// <returns></returns>
		public static DateTime GetLocalTime(this DateTime date, int hourDifference = 1)
        {
            return date.ToUniversalTime().AddHours(hourDifference);
        }

        public static DateTime GetLastDayOfMonth(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));
        }

        public static string FormatDate(this DateTime date, string format = null)
        {
            return date.ToString(format ?? "dd MMM yyyy");
        }

        public static bool IsBefore(this DateTime date, DateTime dateToCompare)
        {
            return date.Date < dateToCompare.Date;
        }

        public static bool IsOnTheSameDateAs(this DateTime date, DateTime dateToCompare)
        {
            return date.Date == dateToCompare.Date;
        }

        public static bool IsAfter(this DateTime date, DateTime dateToCompare)
        {
            return date.Date > dateToCompare.Date;
        }
    }
}
