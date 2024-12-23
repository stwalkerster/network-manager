namespace DnsWebApp.Controllers
{
    using System.Collections.Generic;
    using System.Data;
    using System.Globalization;
    using System.Linq;
    using DnsWebApp.Models.Database;
    using DnsWebApp.Models.ViewModels;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;

    [Route("/report/{action=Index}")]
    public class ReportController : Controller
    {
        private readonly DataContext db;
        private readonly IConfiguration configuration;

        public ReportController(DataContext db, IConfiguration configuration)
        {
            this.db = db;
            this.configuration = configuration;
        }
        
        public IActionResult Index()
        {
            return this.View();
        }

        public IActionResult RenewalCosts()
        {
            var results = new List<ReportResult> {this.GetRenewalAnalysis()};

            return this.View(results);
        }

        
        private ReportResult GetRenewalAnalysis()
        {
            var baseCurrency = this.db.Currencies.FirstOrDefault(
                x => x.Code == this.configuration.GetValue<string>("BaseCurrency"));
           
            var tldData = this.db.RegistrarTldSupport
                .Include(x => x.Registrar)
                .ThenInclude(x => x.Currency)
                .Include(x => x.TopLevelDomain)
                .Select(x => new TldSupportDisplay(x, baseCurrency, this.configuration.GetValue<decimal>("Vat")))
                .ToDictionary(x => x.RegistrarId + "|" + x.TopLevelDomainId);

            var domains = this.db.Domains
                .Include(x => x.TopLevelDomain)
                .Include(x => x.Registrar)
                .Include(x => x.Owner)
                .Where(x => x.TopLevelDomain.Domain != "arpa" && !x.Placeholder)
                .ToList()
                .DistinctBy(x => new {x.TopLevelDomainId, x.Name})
                .ToList();

            var columnHeaders = new List<string>
            {
                "Owner", "TLD", "Domain", "Current registrar", "Renewal price", "Transfer price", "Renewal post transfer", "Recommendation", "Saving", "Expiry date"
            };
            var rows = new List<string[]>();
            var rowsDataValues = new List<string[]>();
            
            foreach (var domain in domains)
            {
                var rowData = new string[columnHeaders.Count];
                var rowDataValues = new string[columnHeaders.Count];

                rowData[0] = domain.Owner?.UserName;
                rowData[1] = domain.TopLevelDomain.Domain;
                rowData[2] = $"{domain.Name}.{domain.TopLevelDomain.Domain}";
                rowData[3] = domain.Registrar?.Name;

                var renewalPrice = decimal.MaxValue;
                TldSupportDisplay renewalObject;
                
                if (domain.Registrar != null)
                {
                    renewalObject = tldData[domain.RegistrarId + "|" + domain.TopLevelDomainId];

                    if (!domain.Registrar.AllowRenewals)
                    {
                        if (renewalObject.RenewalPriceInBaseCurrency.HasValue)
                        {
                            renewalPrice = renewalObject.RenewalPriceInBaseCurrency.Value;
                            rowDataValues[4] = renewalPrice.ToString(CultureInfo.InvariantCulture);
                            rowData[4] = "Prohibited (" + renewalObject.RealRenewalPrice + ")";
                        }
                        else
                        {
                            rowData[4] = "Prohibited";
                        }
                    }
                    else
                    {
                        if (renewalObject.RenewalPriceInBaseCurrency.HasValue)
                        {
                            renewalPrice = renewalObject.RenewalPriceInBaseCurrency.Value;
                            rowDataValues[4] = renewalPrice.ToString(CultureInfo.InvariantCulture);
                            rowData[4] = renewalObject.RealRenewalPrice;
                        }
                        else
                        {
                            rowData[4] = "Unavailable";
                        }
                    }
                }
                else
                {
                    rowData[3] = "Unknown";
                    rowData[7] = "Configure domain registrar";
                    rows.Add(rowData);
                    rowsDataValues.Add(rowDataValues);
                    continue;
                }

                var transferPrice = decimal.MaxValue;
                var tldSupportList = tldData.Values
                    .Where(x => x.TopLevelDomainId == domain.TopLevelDomainId)
                    .Where(x => x.RegistrarId != domain.RegistrarId)
                    .Where(x => x.Registrar?.AllowTransfers == true)
                    .Where(x => x.AllowInboundTransfer)
                    .Where(x => x.TransferPriceInBaseCurrency.HasValue)
                    .ToList();
                
                var transferObject = tldSupportList
                    .OrderBy(x => x.TransferPriceInBaseCurrency)
                    .FirstOrDefault();
                
                if (transferObject == null)
                {                        
                    rowData[4] = "Unavailable";
                }
                else
                {
                    var extraTransferFee = renewalObject.TransferOutInBaseCurrency;
                    var extraTransfer = "";
                    if (extraTransferFee.HasValue)
                    {
                        extraTransfer = $" (+{string.Format(baseCurrency.Symbol, extraTransferFee.Value)} outbound)";
                    }

                    transferPrice = transferObject.TransferPriceInBaseCurrency.Value;
                    rowDataValues[5] = (transferPrice + extraTransferFee.GetValueOrDefault()).ToString(CultureInfo.InvariantCulture);
                    rowData[5] = $"{transferObject.RealTransferPrice} ({transferObject.Registrar.Name}){extraTransfer}";
                    
                    var renewalChange = renewalPrice.CompareTo(transferObject.RenewalPriceInBaseCurrency);
                    string renewalChangeIcon = "";

                    if (renewalPrice != decimal.MaxValue)
                    {
                        if (renewalChange == -1)
                        {
                            renewalChangeIcon =
                                $"⇧ ({string.Format(baseCurrency.Symbol, transferObject.RenewalPriceInBaseCurrency.Value - renewalPrice)})";
                        }
                        else if (renewalChange == 1)
                        {
                            renewalChangeIcon =
                                $"⇩ ({string.Format(baseCurrency.Symbol, transferObject.RenewalPriceInBaseCurrency.Value - renewalPrice)})";
                        }
                        
                        rowDataValues[6] = (transferObject.RenewalPriceInBaseCurrency.Value - renewalPrice).ToString(CultureInfo.InvariantCulture);
                    }

                    rowData[6] = $"{transferObject.RealRenewalPrice} {renewalChangeIcon}";
                }

                if (renewalPrice <= transferPrice && renewalPrice != decimal.MaxValue && !rowData[4].Contains("Prohibited"))
                {
                    rowData[7] = "Renew";

                } 
                else if ((renewalPrice == decimal.MaxValue || rowData[4].Contains("Prohibited")) && transferPrice != decimal.MaxValue)
                {
                    rowData[7] = "Transfer to " + transferObject.Registrar.Name;
                    if (renewalPrice != decimal.MaxValue)
                    {
                        rowData[8] = string.Format(baseCurrency.Symbol, renewalPrice - transferPrice);
                        rowDataValues[8] = (renewalPrice - transferPrice).ToString(CultureInfo.InvariantCulture);
                    }
                }
                else if (transferPrice <= renewalPrice && transferPrice != decimal.MaxValue && renewalPrice != decimal.MaxValue)
                {
                    rowData[7] = "Transfer to " + transferObject.Registrar.Name;
                    rowData[8] = string.Format(baseCurrency.Symbol, renewalPrice - transferPrice);
                    rowDataValues[8] = (renewalPrice - transferPrice).ToString(CultureInfo.InvariantCulture);
                }
                else
                {
                    rowData[6] = "Unknown"; 
                }

                rowData[9] = domain.RegistrationExpiry?.ToString("u");
                rowDataValues[9] = domain.RegistrationExpiry?.ToString("u");
                
                rows.Add(rowData);
                rowsDataValues.Add(rowDataValues);
            }

            return new ReportResult("Transfer/Renewal Recommendations", columnHeaders, rows, rowsDataValues);
        }
        
        private ReportResult GetSqlResult(string query, string title)
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

                    return new ReportResult(title, columns, rows);
                }
            }
        }
    }
}