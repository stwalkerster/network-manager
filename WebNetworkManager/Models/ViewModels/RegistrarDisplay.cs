namespace DnsWebApp.Models.ViewModels
{
    using DnsWebApp.Models.Database;

    public class RegistrarDisplay
    {
        private readonly Currency baseCurrency;
        public Registrar Registrar { get; }

        public RegistrarDisplay(Registrar registrar, Currency baseCurrency)
        {
            this.baseCurrency = baseCurrency;
            this.Registrar = registrar;
        }

        public string PrivacyFee
        {
            get
            {
                if (!this.Registrar.PrivacyFee.HasValue || this.Registrar.Currency == null)
                {
                    return null;
                }
                
                return string.Format(
                    this.baseCurrency.Symbol,
                    this.Registrar.PrivacyFee / this.Registrar.Currency.ExchangeRate * this.baseCurrency.ExchangeRate);
            }
        }
        public string TransferOutFee
        {
            get
            {
                if (!this.Registrar.TransferOutFee.HasValue || this.Registrar.Currency == null)
                {
                    return null;
                }
                return string.Format(
                    this.baseCurrency.Symbol,
                    this.Registrar.TransferOutFee / this.Registrar.Currency.ExchangeRate * this.baseCurrency.ExchangeRate);
            }
        }
    }
}