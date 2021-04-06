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
            var tldStatsQuery = "WITH uqDomain AS (SELECT DISTINCT \"Name\", \"TopLevelDomainId\" FROM \"Domains\")\nSELECT t.\"Domain\", COUNT(1)\nFROM \"TopLevelDomains\" t\nINNER JOIN uqDomain z ON t.\"Id\" = z.\"TopLevelDomainId\"\nGROUP BY t.\"Domain\" ORDER BY t.\"Domain\";";
            var recordStatsQuery = "select r.\"Type\", count(*) from \"Record\" r group by r.\"Type\" order by r.\"Type\"";
            var ownerStatsQuery =
                "SELECT COALESCE(u.\"UserName\", '(unowned)'), COUNT(1) FROM (\nSELECT DISTINCT ON(\"Name\", \"TopLevelDomainId\") \"OwnerId\"\nFROM \"Domains\"\nORDER BY \"Name\", \"TopLevelDomainId\") d\nLEFT JOIN \"AspNetUsers\" u ON d.\"OwnerId\" = u.\"Id\"\nGROUP BY u.\"UserName\";";

            this.ViewBag.TldStats = this.GetDataset(tldStatsQuery);
            this.ViewBag.RecordStats = this.GetDataset(recordStatsQuery);
            this.ViewBag.OwnerStats = this.GetDataset(ownerStatsQuery);
            
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