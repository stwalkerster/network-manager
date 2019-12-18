namespace DnsWebApp.Services
{
    using System;
    using System.Linq;
    using DnsWebApp.Models.Database;
    
    public static class Extensions
    {
        public static void TouchSerialNumber(this Zone zone)
        {
            zone.LastUpdated = DateTime.UtcNow;
            
            var defaultValueStr = DateTime.UtcNow.ToString("yyyyMMdd") + "00";
            var defaultValue = uint.Parse(defaultValueStr);

            if (defaultValue > zone.SerialNumber)
            {
                zone.SerialNumber = defaultValue;
                return;
            }

            zone.SerialNumber += 1;
        }
        
        public static void TouchSerialNumber(this ZoneGroup zoneGroup)
        {
            foreach (var zone in zoneGroup.ZoneGroupMembers.Select(x => x.Zone))
            {
                zone.TouchSerialNumber();
            }
        }
    }
}