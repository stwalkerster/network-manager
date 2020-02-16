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
        private readonly decimal vat;

        public TldSupportDisplay(RegistrarTldSupport tldSupport, Currency baseCurrency, decimal vat)
        {
            this.tldSupport = tldSupport;
            this.baseCurrency = baseCurrency;
            this.vat = vat;
        }

        public override string ToString()
        {
            return $"{nameof(this.Registrar)}: {this.Registrar.Name}, {nameof(this.Domain)}: {this.Domain}";
        }

        public long RegistrarId => this.tldSupport.RegistrarId;
        public Registrar Registrar => this.tldSupport.Registrar;
        public long Id => this.tldSupport.Id;
        public long TopLevelDomainId => this.tldSupport.TopLevelDomainId;

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
        
        public string TransferPrice
        {
            get
            {
                if (!this.tldSupport.TransferPrice.HasValue)
                {
                    return string.Empty;
                }

                if (this.tldSupport.Registrar.Currency != null)
                {
                    return string.Format(
                        this.tldSupport.Registrar.Currency?.Symbol ?? "{0:N2}",
                        this.tldSupport.TransferPrice.Value);
                }

                return this.tldSupport.TransferPrice.Value.ToString(CultureInfo.InvariantCulture);

            }
        }

        public DateTime? RenewalPriceUpdated => this.tldSupport.RenewalPriceUpdated;

        public string RealRenewalPrice => this.RenewalPriceInBaseCurrency.HasValue
            ? string.Format(this.baseCurrency.Symbol, this.RenewalPriceInBaseCurrency)
            : string.Empty;
        
        public string RealTransferPrice => this.TransferPriceInBaseCurrency.HasValue
            ? string.Format(this.baseCurrency.Symbol, this.TransferPriceInBaseCurrency)
            : string.Empty;
        
        public decimal? RenewalPriceInBaseCurrency
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

                var renewal = this.tldSupport.RenewalPrice.Value;

                if (!this.tldSupport.PrivacyIncluded)
                {
                    renewal += this.tldSupport.Registrar.PrivacyFee.GetValueOrDefault(0m);
                }
                
                var convertedValue = renewal / this.tldSupport.Registrar.Currency.ExchangeRate.Value
                                     * this.baseCurrency.ExchangeRate.Value;

                var valueWithTax = convertedValue * (this.tldSupport.Registrar.PricesIncludeVat ? 1m : this.vat);

                return valueWithTax;
            }
        }
        
        public decimal? TransferPriceInBaseCurrency
        {
            get
            {
                if (!this.tldSupport.TransferPrice.HasValue || this.tldSupport.Registrar.Currency == null)
                {
                    return null;
                }

                if (!this.tldSupport.Registrar.Currency.ExchangeRate.HasValue
                    || !this.baseCurrency.ExchangeRate.HasValue)
                {
                    return null;
                }

                var transferPrice = this.tldSupport.TransferPrice.Value;
                if (!this.tldSupport.TransferIncludesRenewal)
                {
                    if (!this.tldSupport.RenewalPrice.HasValue)
                    {
                        return null;
                    }

                    transferPrice += this.tldSupport.RenewalPrice.Value;
                }
                
                if (!this.tldSupport.PrivacyIncluded)
                {
                    transferPrice += this.tldSupport.Registrar.PrivacyFee.GetValueOrDefault(0m);
                }
                
                var convertedValue = transferPrice / this.tldSupport.Registrar.Currency.ExchangeRate.Value
                                     * this.baseCurrency.ExchangeRate.Value;

                var valueWithTax = convertedValue * (this.tldSupport.Registrar.PricesIncludeVat ? 1m : 1.2m);

                return valueWithTax;
            }
        }

        public decimal? TransferOutInBaseCurrency
        {
            get
            {
                if (!this.tldSupport.Registrar.Currency.ExchangeRate.HasValue
                    || !this.baseCurrency.ExchangeRate.HasValue)
                {
                    return null;
                }
                
                if (this.tldSupport.Registrar.TransferOutFee.HasValue)
                {
                    return this.tldSupport.Registrar.TransferOutFee.Value / this.tldSupport.Registrar.Currency.ExchangeRate.Value
                                         * this.baseCurrency.ExchangeRate.Value;
                }

                return null;
            }
        }

        public string TotalYearlyCost => this.RenewalPriceInBaseCurrency.HasValue
            ? string.Format(this.baseCurrency.Symbol, this.EnabledZones * this.RenewalPriceInBaseCurrency)
            : string.Empty;
    }
}