namespace DnsWebApp.Services
{
    using System;

    public class FormattingService
    {
        public string AsReadableDateTime(DateTime date)
        {
            return date.ToString("yyyy-MM-dd HH:mm:ss");
        }
        
        public string AsReadableDate(DateTime date)
        {
            return date.ToString("yyyy-MM-dd");
        }
        
        public string AsReadableDate(DateTime? date)
        {
            if (date.HasValue)
            {
                return date.Value.ToString("yyyy-MM-dd");
            }

            return ("(unknown date)");
        }

        public string AsReadableTimespan(uint seconds)
        {
            var ts = new TimeSpan(0, 0, 0, (int) seconds);

            if ((ts.TotalDays / 7) % 1 == 0)
            {
                return (ts.TotalDays /7) + "w";
            }
            
            if (ts.TotalDays % 1 == 0)
            {
                return ts.TotalDays + "d";
            }
            
            if (ts.TotalHours % 1 == 0)
            {
                return ts.TotalHours + "h";
            }
            
            if (ts.TotalMinutes % 1 == 0)
            {
                return ts.TotalMinutes + "m";
            }

            return seconds + "s";
        }
    }
}