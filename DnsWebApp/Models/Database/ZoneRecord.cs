namespace LdapDnsWebApp.Models.Database
{
    using System.ComponentModel.DataAnnotations;

    public class ZoneRecord
    {
        public long Id { get; set; }
        [Required]
        public Zone Zone { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public RecordClass Class { get; set; }
        [Required]
        public RecordType Type { get; set; }
        public int? TimeToLive { get; set; }
        [Required]
        public string Value { get; set; }
    }
}