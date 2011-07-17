using System;
using System.Globalization;

namespace FarleyFile
{
    public static class FormatEvil
    {
        public static string OffsetUtc(DateTimeOffset time)
        {
            if (time == default(DateTimeOffset))
                return "";
            

            var now = DateTimeOffset.UtcNow;
            var offset = now - time;

            if (Math.Abs(offset.TotalDays) > 7)
            {
                return time.ToLocalTime().ToString("MMMM dd, yyyy", CultureInfo.InvariantCulture);
            }

            if (offset > TimeSpan.Zero)
            {
                return PositiveTimeSpan(offset) + " ago";
            }
            return PositiveTimeSpan(-offset) + " from now";
        }


        static string PositiveTimeSpan(TimeSpan timeSpan)
        {
            const int second = 1;
            const int minute = 60 * second;
            const int hour = 60 * minute;
            const int day = 24 * hour;
            const int month = 30 * day;

            double delta = timeSpan.TotalSeconds;

            if (delta < 5 * second) return "moments";
            if (delta < 1 * minute) return timeSpan.Seconds + " seconds";
            if (delta < 2 * minute) return "a minute";
            if (delta < 50 * minute) return timeSpan.Minutes + " minutes";
            if (delta < 70 * minute) return "an hour";
            if (delta < 2 * hour) return (int)timeSpan.TotalMinutes + " minutes";
            if (delta < 24 * hour) return timeSpan.Hours + " hours";
            if (delta < 48 * hour) return (int)timeSpan.TotalHours + " hours";
            if (delta < 30 * day) return timeSpan.Days + " days";

            if (delta < 12 * month)
            {
                var months = (int)Math.Floor(timeSpan.Days / 30.0);
                return months <= 1 ? "one month" : months + " months";
            }

            var years = (int)Math.Floor(timeSpan.Days / 365.0);
            return years <= 1 ? "one year" : years + " years";
        }

    }
}