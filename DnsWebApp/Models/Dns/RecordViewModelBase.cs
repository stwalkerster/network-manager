namespace DnsWebApp.Models.Dns
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using DnsWebApp.Models.Database;
    using Microsoft.Extensions.WebEncoders.Testing;

    public class RecordViewModelBase
    {
        public Record Record { get; }

        public virtual string Name { get => this.Record.Name; set => this.Record.Name = value; }  
        public virtual string Value { get => this.Record.Value; set => this.Record.Value = value; }  
        
        [Display(Name = "Time to Live")]
        public virtual uint? TimeToLive { get => this.Record.TimeToLive; set => this.Record.TimeToLive = value; }
        
        public RecordType Type { get => this.Record.Type; set => this.Record.Type = value; }
        public RecordClass Class { get => this.Record.Class; set => this.Record.Class = value; }

        public long Id => this.Record.Id;
        public long? ZoneGroupId { get => this.Record.ZoneGroupId; set => this.Record.ZoneGroupId = value; }
        public long? ZoneId { get => this.Record.ZoneId; set => this.Record.ZoneId = value; }

        protected int NameMaxParts { private get; set; } = 1;

        private List<string> valueParts, nameParts;
        
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

        [Obsolete]
        protected List<string> Parse(bool name = false)
        {
            return this.Parse(name, true);
        }
        
        private List<string> Parse(bool name, bool @internal)
        {
            var recordValue = name ? this.Record.Name : this.Record.Value;

            if (recordValue == null)
            {
                recordValue = string.Empty;
            }
            
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
            var partsList = name ? this.nameParts : this.valueParts;
            
            if (partsList == null)
            {
                partsList = this.Parse(name, true);
            }

            if (partsList.Count <= index)
            {
                while (partsList.Count <= index)
                {
                    partsList.Add(string.Empty);
                }
            }
            
            partsList[index] = value;

            var resultant = string.Join(name ? "." : " ", partsList.Where(x => x != null).Select(x => x.Contains(" ") ? $"\"{x}\"" : x));

            if (name)
            {
                this.nameParts = partsList;
                this.Record.Name = resultant;
            }
            else
            {
                this.valueParts = partsList;
                this.Record.Value = resultant;
            }
        }

        protected string Get(int index, bool name = false)
        {
            var partsList = name ? this.nameParts : this.valueParts;
            
            if (partsList == null)
            {
                partsList = this.Parse(name, true);
                
                if (name)
                {
                    this.nameParts = partsList;
                }
                else
                {
                    this.valueParts = partsList;
                }
            }

            if (partsList.Count > index)
            {
                return partsList[index];
            }
            else
            {
                return "";
            }
        }
    }
}