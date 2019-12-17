namespace DnsWebApp.Models.Dns
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DnsWebApp.Models.Database;

    public class RecordViewModelBase
    {
        public Record Record { get; }

        public virtual string Name { get => this.Record.Name; set => this.Record.Name = value; }  
        public virtual string Value { get => this.Record.Value; set => this.Record.Value = value; }  
        public virtual uint? TimeToLive { get => this.Record.TimeToLive; set => this.Record.TimeToLive = value; }
        public RecordType Type { get => this.Record.Type; set => this.Record.Type = value; }
        public RecordClass Class { get => this.Record.Class; set => this.Record.Class = value; }

        public long Id => this.Record.Id;
        public long? ZoneGroupId { get => this.Record.ZoneGroupId; set => this.Record.ZoneGroupId = value; }
        public long? ZoneId { get => this.Record.ZoneId; set => this.Record.ZoneId = value; }

        protected int NameMaxParts { private get; set; } = 1;
        protected int ValueParts { private get; set; } = 1;
        
        protected RecordViewModelBase(Record record, RecordType type)
        {
            this.Record = record;

            if (record == null)
            {
                this.Record = new Record {Type = type, Class = RecordClass.IN};
            }
            
            if (this.Record.Type != type)
            {
                throw new ArgumentOutOfRangeException(nameof(record));
            }
        }
        
        protected List<string> Parse(bool name = false)
        {
            var recordValue = name ? this.Record.Name : this.Record.Value;
            
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
                this.Record.Name = resultant;
            }
            else
            {
                this.Record.Value = resultant;
            }
        }
    }
}