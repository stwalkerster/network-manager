namespace DnsWebApp.Models
{
    public enum SshfpAlgorithm
    {
        RSA = 1,
        DSA = 2,
        ECDSA = 3,
        // ReSharper disable once InconsistentNaming
        Ed25519 = 4
    }
}