namespace DnsWebApp.Models.Dns
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DnsWebApp.Models.Database;

    public class RecordViewModelBase
    {
        private readonly Record record;

        public virtual string Name { get => this.record.Name; set => this.record.Name = value; }  
        public virtual string Value { get => this.record.Value; set => this.record.Value = value; }  
        public virtual uint? TimeToLive { get => this.record.TimeToLive; set => this.record.TimeToLive = value; }
        public RecordType Type { get => this.record.Type; }

        public long Id => this.record.Id;
        public long? ZoneGroupId => this.record.ZoneGroupId;

        protected int NameMaxParts { private get; set; } = 1;
        
        protected RecordViewModelBase(Record record, RecordType type)
        {
            this.record = record;
            if (record.Type != type)
            {
                throw new ArgumentOutOfRangeException(nameof(record));
            }
        }
        
        protected List<string> Parse(bool name = false)
        {
            var recordValue = name ? this.record.Name : this.record.Value;
            
            var items = new List<string>();
            var current = string.Empty;
            var inQuote = false;

            var separator = name ? '.' : ' ';

            foreach (var c in recordValue)
            {
                if (name && items.Count + 1 == this.NameMaxParts)
                {
                    current += c;
                    continue;
                }
                
                if (c == '"')
                {
                    inQuote = !inQuote;
                    continue;
                }

                if (c == separator)
                {
                    items.Add(current);
                    current = string.Empty;
                    continue;
                }

                current += c;
            }

            if (!string.IsNullOrWhiteSpace(current))
            {
                items.Add(current);
            }

            return items;
        }

        protected void Set(int index, string value, bool name = false)
        {
            var parts = this.Parse(name);
            parts[index] = value;
            var resultant = string.Join(" ", parts.Select(x => x.Contains(" ") ? $"\"{x}\"" : x));

            if (name)
            {
                this.record.Name = resultant;
            }
            else
            {
                this.record.Value = resultant;
            }
        }
    }
}