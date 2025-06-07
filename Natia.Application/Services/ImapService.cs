using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using Natia.Application.Contracts;
using Natia.Application.Dtos;

namespace Natia.Application.Services;

public class ImapService : IImapServices
{

    public async Task<List<MaillMessageDto>> CheckForNewMessage()
    {
        using (var client = new ImapClient())
        {
            try
            {
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                await client.ConnectAsync("imap.gmail.com", 993, true);

                await client.AuthenticateAsync("globaltvmanagment@gmail.com", "zocg vzno fiji jzge");

                var inbox = client.Inbox;
                await inbox.OpenAsync(MailKit.FolderAccess.ReadWrite);

                var blacklistedSenders = new List<string> { "Google", "Spam Sender", "NoReply", "Admin", "Google Community Team" };

                var unreadIds = await inbox.SearchAsync(SearchQuery.NotSeen);
                List<MaillMessageDto> mails = new List<MaillMessageDto>();

                foreach (var uniqueId in unreadIds)
                {
                    var message = await inbox.GetMessageAsync(uniqueId);

                    string senderName = message.From.Mailboxes.FirstOrDefault()?.Name??"";

                    if (message.Subject.ToLower().Contains("natia") || message.Subject.ToLower().Contains("ნათია"))
                    {
                        continue;
                    }
                    if (!string.IsNullOrEmpty(senderName) && blacklistedSenders.Contains(senderName))
                    {
                        await inbox.AddFlagsAsync(uniqueId, MessageFlags.Seen, true);
                        continue;
                    }

                    mails.Add(new MaillMessageDto
                    {
                        Date = message.Date.ToString(),
                        Body = message.TextBody ?? message.HtmlBody,
                        Name = senderName,
                        Subject = message.Subject,
                    });

                    await inbox.AddFlagsAsync(uniqueId, MessageFlags.Seen, true);
                }

                await client.DisconnectAsync(true);

                return mails;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                throw;
            }
        }
    }



    public async Task<List<MaillMessageDto>> CheckforReplay()
    {
        using (var client = new ImapClient())
        {
            try
            {
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                await client.ConnectAsync("imap.gmail.com", 993, true);

                await client.AuthenticateAsync("globaltvmanagment@gmail.com", "zocg vzno fiji jzge");

                var inbox = client.Inbox;
                await inbox.OpenAsync(MailKit.FolderAccess.ReadWrite);

                var blacklistedSenders = new List<string> { "Google", "Spam Sender", "NoReply", "Admin", "Google Community Team" };

                var unreadIds = await inbox.SearchAsync(SearchQuery.NotSeen);
                List<MaillMessageDto> mails = new List<MaillMessageDto>();

                foreach (var uniqueId in unreadIds)
                {
                    var message = await inbox.GetMessageAsync(uniqueId);

                    string senderName = message.From.Mailboxes.FirstOrDefault()?.Name ?? "";
                    string mail = message.From.Mailboxes.FirstOrDefault()?.Address ?? "";
                    if (!message.Subject.ToLower().Contains("natia") && !message.Subject.ToLower().Contains("ნათია"))
                    {
                        continue;
                    }
                    if (!string.IsNullOrEmpty(senderName) && blacklistedSenders.Contains(senderName))
                    {
                        await inbox.AddFlagsAsync(uniqueId, MessageFlags.Seen, true);
                        continue;
                    }

                    mails.Add(new MaillMessageDto
                    {
                        Date = message.Date.ToString(),
                        Body = message.TextBody ?? message.HtmlBody,
                        Name = senderName,
                        Subject = message.Subject,
                        Email = mail,
                    });

                    await inbox.AddFlagsAsync(uniqueId, MessageFlags.Seen, true);
                }

                await client.DisconnectAsync(true);

                return mails;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                throw;
            }
        }
    }
}
