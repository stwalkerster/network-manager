namespace DnsWebApp.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using DnsWebApp.Models.Database;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;

    [Route("/report/{action=Index}")]
    public class ReportController : Controller
    {
        private readonly DataContext db;

        public ReportController(DataContext db)
        {
            this.db = db;
        }
        
        public IActionResult Index()
        {
            return this.View();
        }

        public IActionResult RenewalCosts()
        {
            var registrarSnippet = this.db.Registrar
                .Include(x => x.RegistrarTldSupports)
                .Where(x => x.RegistrarTldSupports != null && x.RegistrarTldSupports.Count > 0)
                .OrderBy(x => x.Name)
                .Select(x => string.Format("\"{0}\" NUMERIC", x.Name))
                .ToList()
                .Aggregate("\"Domain\" TEXT", (a, c) => a + ", " + c);

            var commandCore = "select '.' || tld.\"Domain\", registrar.\"Name\", \n       tldSupport.\"RenewalPrice\" * coalesce(registrarCcy.\"ExchangeRate\", -1) * baseCcy.\"ExchangeRate\"\nfrom \"TopLevelDomains\" tld\n         cross join \"Registrar\" registrar\n         left join \"RegistrarTldSupport\" tldSupport on tld.\"Id\" = tldSupport.\"TopLevelDomainId\" and registrar.\"Id\" = tldSupport.\"RegistrarId\"\n         left join \"Currencies\" registrarCcy on registrar.\"CurrencyId\" = registrarCcy.\"Id\"\n         cross join \"Currencies\" baseCcy\nwhere baseCcy.\"Code\" = 'GBP'\norder by 1,2\n";
            var costsPivot = string.Format(
                "SELECT * FROM public.crosstab($q$ {0} $q$) AS final_result({1})",
                commandCore,
                registrarSnippet);


            var renewal = @"
with d as (
    select distinct z.""Id""                                                             as zoneid,
                    z.""Name""                                                           as zonename,
                    z.""TopLevelDomainId""                                               as tldid,
                    z.""OwnerId""                                                        as owner,
                    tld.""Domain""                                                       as tld,
                    r.""Id""                                                             as regid,
                    r.""Name""                                                           as regname,
                    min(RTS.""RenewalPrice"") over (partition by RTS.""TopLevelDomainId"") as MinRenewalPrice,
                    rrtld.""RenewalPrice""                                               as actualrenewalprice
    from ""Zones"" z
             inner join ""TopLevelDomains"" tld on z.""TopLevelDomainId"" = tld.""Id""
             inner join ""RegistrarTldSupport"" RTS on tld.""Id"" = RTS.""TopLevelDomainId""
             inner join ""Registrar"" r on z.""RegistrarId"" = r.""Id""
             inner join ""RegistrarTldSupport"" rrtld
                        on r.""Id"" = rrtld.""RegistrarId"" and rrtld.""TopLevelDomainId"" = z.""TopLevelDomainId""
)
select --d.zoneid,
       d.zonename || '.' || d.tld as ""Domain"",
       d.regname as ""Current registrar"",
       to_char(d.actualrenewalprice, 'LFM9999990.00') as ""Current Price"",
       to_char(d.MinRenewalPrice, 'LFM9999990.00') as ""Min Price"",       
       to_char(d.actualrenewalprice - d.MinRenewalPrice, 'LFM9999990.00') as ""Saving"",
       to_char(SUM(d.actualrenewalprice - d.MinRenewalPrice) OVER (PARTITION BY d.owner), 'LFM9999990.00') as ""Total Saving"",
       prr.""Name"" as ""New registrar"",
    --   d.owner,
       u.""UserName"" as ""Owner""
from d
         inner join ""RegistrarTldSupport"" prtld
                    on prtld.""RenewalPrice"" = d.MinRenewalPrice and prtld.""TopLevelDomainId"" = d.tldid
         inner join ""Registrar"" prr on prtld.""RegistrarId"" = prr.""Id""
         inner join ""AspNetUsers"" u on u.""Id"" = d.owner
where d.MinRenewalPrice < d.actualrenewalprice
";

            var results = new List<Tuple<List<string>, List<string[]>>>
                {this.GetSqlResult(costsPivot), this.GetSqlResult(renewal)};

            return this.View(results);
        }

        private Tuple<List<string>, List<string[]>> GetSqlResult(string query)
        {
            using (var command = this.db.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = query;
                command.CommandType = CommandType.Text;

                this.db.Database.OpenConnection();

                using (var result = command.ExecuteReader())
                {
                    var columns = new List<string>();
                    for (var i = 0; i < result.FieldCount; i++)
                    {
                        columns.Add(result.GetName(i));
                    }

                    var rows = new List<string[]>();

                    while (result.Read())
                    {
                        var data = new string[result.FieldCount];
                        for (var i = 0; i < result.FieldCount; i++)
                        {
                            data[i] = result[i].ToString();
                        }

                        rows.Add(data);
                    }

                    return new Tuple<List<string>, List<string[]>>(columns, rows);
                }
            }
        }
    }
}