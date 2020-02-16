namespace DnsWebApp.Models.ViewModels
{
    using System;
    using System.Globalization;
    using System.Linq;
    using DnsWebApp.Models.Database;

    public class TldSupportDisplay
    {
        private readonly RegistrarTldSupport tldSupport;
        private readonly Currency baseCurrency;

        public TldSupportDisplay(RegistrarTldSupport tldSupport, Currency baseCurrency)
        {
            this.tldSupport = tldSupport;
            this.baseCurrency = baseCurrency;
        }

        public long RegistrarId => this.tldSupport.RegistrarId;
        public long Id => this.tldSupport.Id;

        public string Domain => this.tldSupport.TopLevelDomain.Domain;
        public int EnabledZones => this.tldSupport.TopLevelDomain.Zones.Where(x => x.RegistrarId == this.tldSupport.RegistrarId && x.Enabled).Select(x => x.Name).Distinct().Count();
        public int DisabledZones => this.tldSupport.TopLevelDomain.Zones.Where(x => x.RegistrarId == this.tldSupport.RegistrarId && !x.Enabled).Select(x => x.Name).Distinct().Count();
        
        public string RenewalPrice
        {
            get
            {
                if (!this.tldSupport.RenewalPrice.HasValue)
                {
                    return string.Empty;
                }

                if (this.tldSupport.Registrar.Currency != null)
                {
                    return string.Format(
                        this.tldSupport.Registrar.Currency?.Symbol ?? "{0:N2}",
                        this.tldSupport.RenewalPrice.Value);
                }

                return this.tldSupport.RenewalPrice.Value.ToString(CultureInfo.InvariantCulture);

            }
        }

        public DateTime? RenewalPriceUpdated => this.tldSupport.RenewalPriceUpdated;

        public string RealRenewalPrice => this.RenewalPriceInBaseCurrency.HasValue
            ? string.Format(this.baseCurrency.Symbol, this.RenewalPriceInBaseCurrency)
            : string.Empty;
        
        private decimal? RenewalPriceInBaseCurrency
        {
            get
            {
                if (!this.tldSupport.RenewalPrice.HasValue || this.tldSupport.Registrar.Currency == null)
                {
                    return null;
                }

                if (!this.tldSupport.Registrar.Currency.ExchangeRate.HasValue
                    || !this.baseCurrency.ExchangeRate.HasValue)
                {
                    return null;
                }

                var convertedValue = this.tldSupport.RenewalPrice.Value / this.tldSupport.Registrar.Currency.ExchangeRate.Value
                                     * this.baseCurrency.ExchangeRate.Value;

                var valueWithTax = convertedValue * (this.tldSupport.Registrar.PricesIncludeVat ? 1m : 1.2m);

                return valueWithTax;
            }
        }

        public string TotalYearlyCost => this.RenewalPriceInBaseCurrency.HasValue
            ? string.Format(this.baseCurrency.Symbol, this.EnabledZones * this.RenewalPriceInBaseCurrency)
            : string.Empty;
    }
}