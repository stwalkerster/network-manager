namespace DnsWebApp.Services
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net.Sockets;
    using DnsWebApp.Models;
    using DnsWebApp.Models.Database;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;

    public class WhoisService
    {
        private readonly DataContext db;
        private readonly ILogger<WhoisService> logger;

        public WhoisService(DataContext db, ILogger<WhoisService> logger)
        {
            this.db = db;
            this.logger = logger;
        }

        public WhoisResult GetWhoisData(long zoneId)
        {
            var zone = this.db.Zones.Include(x => x.TopLevelDomain).FirstOrDefault(x => x.Id == zoneId);

            if (zone == null)
            {
                return new WhoisResult();
            }
            
            return this.GetWhoisData(zone);
        }

        public WhoisResult GetWhoisData(Zone zone)
        {
            this.db.Entry(zone).Reference(x => x.TopLevelDomain).Load();
            var server = zone.TopLevelDomain.WhoisServer;
            var domain = zone.Name + "." + zone.TopLevelDomain.Domain;

            if (server == null)
            {
                return new WhoisResult();
            }

            var tcpClient = new TcpClient(server, 43);
            var stream = tcpClient.GetStream();
            var sr = new StreamReader(stream);
            var sw = new StreamWriter(stream) {NewLine = "\r\n"};
            sw.WriteLine(domain);
            sw.Flush();
            var data = sr.ReadToEnd();
            tcpClient.Close();
            sw.Dispose();
            sr.Dispose();
            tcpClient.Dispose();

            var expiry = data.Split(new[] {'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries)
                .Where(x => x.ToLower(CultureInfo.InvariantCulture).Contains("expiry date:"))
                .Select(x => x.Split(':', 2)[1].Trim())
                .Select(DateTime.Parse)
                .FirstOrDefault();

            return new WhoisResult {Expiry = expiry};
        }

        public void UpdateExpiryAttributes(IEnumerable<Zone> zones)
        {
            bool changed = false;
            
            foreach (var zone in zones)
            {
                try
                {
                    // update if:
                    //   * no exiry date set
                    //   * not refreshed
                    //   * < 14 days to expiry, refresh every 6 hours
                    //   * every 7 days.
                    if (!zone.RegistrationExpiry.HasValue
                        || !zone.WhoisLastUpdated.HasValue
                        || ((DateTime.UtcNow - zone.RegistrationExpiry.Value).TotalDays < 14
                            && (DateTime.UtcNow - zone.WhoisLastUpdated.Value).TotalHours > 6)
                        || (DateTime.UtcNow - zone.WhoisLastUpdated.GetValueOrDefault(DateTime.MinValue)).TotalDays > 7
                    )
                    {
                        var whoisResult = this.GetWhoisData(zone);

                        if (whoisResult.Expiry.HasValue)
                        {
                            zone.RegistrationExpiry = whoisResult.Expiry;
                            zone.WhoisLastUpdated = DateTime.UtcNow;
                            changed = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    this.logger.LogError(
                        ex,
                        $"Exception encountered updating whois data for zone {zone.Id}:{zone.Name}");
                }
            }

            if (changed)
            {
                this.db.SaveChanges();
            }
        }
    }
}