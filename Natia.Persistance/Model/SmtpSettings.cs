namespace Natia.Persistance.Model
{
    public class SmtpSettings
    {
        public string? Server { get; set; }
        public int Port { get; set; }
        public string? SenderEmail { get; set; }
        public string? SenderPassword { get; set; }
    }
}
