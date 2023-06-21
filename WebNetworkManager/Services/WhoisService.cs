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

        public WhoisResult GetWhoisData(long domainId)
        {
            var domain = this.db.Domains.Include(x => x.TopLevelDomain).FirstOrDefault(x => x.Id == domainId);

            if (domain == null)
            {
                return new WhoisResult();
            }
            
            return this.GetWhoisData(domain);
        }

        public WhoisResult GetWhoisData(Domain domain)
        {
            this.db.Entry(domain).Reference(x => x.TopLevelDomain).Load();
            var server = domain.TopLevelDomain.WhoisServer;
            var domainName = domain.Name + "." + domain.TopLevelDomain.Domain;

            if (server == null)
            {
                return new WhoisResult();
            }

            var tcpClient = new TcpClient(server, 43);
            var stream = tcpClient.GetStream();
            var sr = new StreamReader(stream);
            var sw = new StreamWriter(stream) {NewLine = "\r\n"};
            sw.WriteLine(domainName);
            sw.Flush();
            var data = sr.ReadToEnd();
            tcpClient.Close();
            sw.Dispose();
            sr.Dispose();
            tcpClient.Dispose();

            var expiry = data.Split(new[] {'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries)
                .Where(x => x.ToLower(CultureInfo.InvariantCulture).Contains("expiry date:"))
                .Select(x => x.Split(':', 2)[1].Trim())
                .Select(
                    d => domain.TopLevelDomain.WhoisExpiryDateFormat != null
                        ? DateTime.ParseExact(
                            d,
                            domain.TopLevelDomain.WhoisExpiryDateFormat,
                            CultureInfo.InvariantCulture)
                        : DateTime.Parse(d))
                .FirstOrDefault();

            return new WhoisResult {Expiry = expiry};
        }

        public void UpdateExpiryAttributes(IEnumerable<Domain> domains)
        {
            bool changed = false;
            
            foreach (var domain in domains)
            {
                try
                {
                    // update if:
                    //   * no exiry date set
                    //   * not refreshed
                    //   * < 14 days to expiry, refresh every 6 hours
                    //   * every 7 days.
                    if (!domain.RegistrationExpiry.HasValue
                        || !domain.WhoisLastUpdated.HasValue
                        || ((DateTime.UtcNow - domain.RegistrationExpiry.Value).TotalDays < 14
                            && (DateTime.UtcNow - domain.WhoisLastUpdated.Value).TotalHours > 6)
                        || (DateTime.UtcNow - domain.WhoisLastUpdated.GetValueOrDefault(DateTime.MinValue)).TotalDays > 7
                    )
                    {
                        var whoisResult = this.GetWhoisData(domain);

                        if (whoisResult.Expiry.HasValue)
                        {
                            domain.RegistrationExpiry = whoisResult.Expiry.Value.ToUniversalTime();
                            domain.WhoisLastUpdated = DateTime.UtcNow;
                            changed = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    this.logger.LogError(
                        ex,
                        "Exception encountered updating whois data for domain {Id}:{Name}", domain.Id, domain.Name);
                }
            }

            if (changed)
            {
                this.db.SaveChanges();
            }
        }
    }
}