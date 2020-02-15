namespace DnsWebApp.Validation
{
    using System.ComponentModel.DataAnnotations;

    public class NotEqualAttribute : ValidationAttribute
    {
        public long Value { get; }

        public NotEqualAttribute(long value)
        {
            this.Value = value;
        }

        public string GetErrorMessage() => $"This value must be defined";

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (this.Value == (long) value)
            {
                return new ValidationResult(this.GetErrorMessage());
            }
            
            return ValidationResult.Success;
        }
    }
}