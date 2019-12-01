namespace LdapDnsWebApp.Services
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net.Sockets;
    using LdapDnsWebApp.Models;
    using Microsoft.Extensions.Configuration;

    public class WhoisService
    {
        private Dictionary<string, string> whoisServers;

        public WhoisService(IConfiguration config)
        {
            this.whoisServers = config.GetSection("whoisServers").GetChildren().ToDictionary(x => x.Key, x => x.Value);
        }

        public WhoisResult GetWhoisData(string domain)
        {
            var server = this.whoisServers.Where(x => domain.EndsWith("." + x.Key))
                .OrderByDescending(x => x.Key.Length)
                .FirstOrDefault()
                .Value;

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