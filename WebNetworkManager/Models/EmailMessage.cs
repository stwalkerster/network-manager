namespace DnsWebApp.Models
{
    using System.Collections.Generic;
    using System.Linq;
    using MimeKit;

    public class EmailMessage
    {
        public EmailMessage(string to, string toName, string subject, string content)
        {
            this.To = new[] { to }.Select(x => new MailboxAddress(toName, x)).ToList();
            this.Subject = subject;
            this.Content = content;        
        }

        public List<MailboxAddress> To { get; set; }    
        public string Subject { get; set; }
        public string Content { get; set; }
    }
}