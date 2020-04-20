namespace DnsWebApp.Services
{
    using DnsWebApp.Models;
    using DnsWebApp.Models.Config;
    using MailKit.Net.Smtp;
    using MimeKit;

    public class EmailSender : IEmailSender
    {
        private readonly EmailConfig config;
 
        public EmailSender(EmailConfig emailConfig)
        {
            this.config = emailConfig;
        }
        
        public void Send(EmailMessage emailMessage)
        {
            var mimeMessage = CreateMimeMessage(emailMessage);
 
            Send(mimeMessage);
        }
        
        private MimeMessage CreateMimeMessage(EmailMessage emailMessage)
        {
            var mimeMessage = new MimeMessage();
            mimeMessage.From.Add(new MailboxAddress(config.From));
            mimeMessage.To.AddRange(emailMessage.To);
            mimeMessage.Subject = emailMessage.Subject;
            mimeMessage.Body = new TextPart(MimeKit.Text.TextFormat.Text) { Text = emailMessage.Content };
 
            return mimeMessage;
        }

        private void Send(MimeMessage message)
        {
            using var client = new SmtpClient();
            
            try
            {
                client.ServerCertificateValidationCallback = (sender, certificate, chain, errors) => true;
                client.Connect(this.config.SmtpServer, this.config.Port, this.config.UseSsl);

                if (!string.IsNullOrWhiteSpace(this.config.UserName))
                {
                    client.Authenticate(this.config.UserName, this.config.Password);
                }

                client.Send(message);
            }
            finally
            {
                client.Disconnect(true);
                client.Dispose();
            }
        }
    }

    public interface IEmailSender
    {
        void Send(EmailMessage emailMessage);
    }
}