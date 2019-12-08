namespace DnsWebApp.Models.Database
{
    using System.ComponentModel.DataAnnotations;

    public class Record
    {
        public long Id { get; set; }
        
        public string Name { get; set; }
        
        [Required]
        public RecordClass Class { get; set; }
        
        [Required]
        public RecordType Type { get; set; }
        
        public uint? TimeToLive { get; set; }
        
        [Required]
        public string Value { get; set; }
    }
}