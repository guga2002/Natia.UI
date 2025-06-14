namespace Natia.Persistance.Interface
{
    public interface ISmtpClientRepository
    {
        Task SendMessage(string body);

        string BuildHtmlMessage(string message, string stackTrace);

        Task SendMessage(string body, string to, string Subject);
    }
}
