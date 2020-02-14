namespace DnsWebApp.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using DnsWebApp.Models.Database;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;

    public class HomeController : Controller
    {
        private readonly DataContext db;

        public HomeController(DataContext db)
        {
            this.db = db;
        }
        
        // GET
        public IActionResult Index()
        {
            var tldStatsQuery = "select tld.\"Domain\", count(*)\nfrom \"TopLevelDomains\" tld inner join \"Zones\" z on tld.\"Id\" = z.\"TopLevelDomainId\" \ngroup by tld.\"Domain\" order by tld.\"Domain\"";
            var recordStatsquery = "select r.\"Type\", count(*) from \"Record\" r group by r.\"Type\" order by r.\"Type\"";

            this.ViewBag.TldStats = this.GetDataset(tldStatsQuery);
            this.ViewBag.RecordStats = this.GetDataset(recordStatsquery);
            
            return View();
        }

        private Tuple<List<string>, List<long>> GetDataset(string query)
        {
            Tuple<List<string>, List<long>> data;
            using (var command = this.db.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = query;
                command.CommandType = CommandType.Text;

                this.db.Database.OpenConnection();

                using var result = command.ExecuteReader();
                var rows = new Dictionary<string, long>();
                while (result.Read())
                {
                    rows.Add(result[0].ToString(), (long) result[1]);
                }

                data = new Tuple<List<string>, List<long>>(rows.Keys.ToList(), rows.Values.ToList());
            }

            return data;
        }
    }
}