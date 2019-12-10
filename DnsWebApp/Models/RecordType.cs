// ReSharper disable InconsistentNaming
namespace DnsWebApp.Models
{
    public enum RecordType
    {
        A = 1,
        AAAA = 28,
        CAA = 257,
        CNAME = 5,
        MX = 15,
        NS = 2,
        PTR = 12,
        SOA = 6,
        SRV = 33,
        SSHFP = 44,
        TXT = 16
    }
}