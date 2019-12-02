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

    public class WhoisService
    {
        private readonly DataContext db;

        public WhoisService(DataContext db)
        {
            this.db = db;
        }

        public WhoisResult GetWhoisData(long zoneId)
        {
            var zone = this.db.Zones.Include(x => x.TopLevelDomain).FirstOrDefault(x => x.Id == zoneId);
            
            var server = zone.TopLevelDomain.WhoisServer;
            var domain = zone.Name + "." + zone.TopLevelDomain.Domain;
            
            if (server == null)
            {
                return new WhoisResult();
            }

            var tcpClient = new TcpClient(server, 43);
            var stream = tcpClient.GetStream();
            var sr = new StreamReader(stream);
            var sw = new StreamWriter(stream);
            sw.NewLine = "\r\n";
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
    }
}