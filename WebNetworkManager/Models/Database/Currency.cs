namespace DnsWebApp.Models.Database
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class Currency
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        
        public string Symbol { get; set; }
        
        [Display(Name = "FX rate")]
        public decimal? ExchangeRate { get; set; }
        
        [Display(Name = "Last update")]
        public DateTime? ExchangeRateUpdated { get; set; }
    }
}