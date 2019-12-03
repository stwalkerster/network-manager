namespace DnsWebApp.Services
{
    using System;

    public class FormattingService
    {
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
    }
}