// Models/NotificationConfig.cs
namespace QuestGuildTerminal
{
    public class NotificationConfig
    {
        public EmailConfig Email { get; set; }
        public SmsConfig Sms { get; set; }
        
        public bool IsEmailEnabled => Email != null && !string.IsNullOrEmpty(Email.ApiKey);
        public bool IsSmsEnabled => Sms != null && !string.IsNullOrEmpty(Sms.AccountSid) && !string.IsNullOrEmpty(Sms.AuthToken);
    }

    public class EmailConfig
    {
        public string ApiKey { get; set; }
        public string FromEmail { get; set; }
        public string FromName { get; set; } = "Quest Guild Terminal";
    }

    public class SmsConfig
    {
        public string AccountSid { get; set; }
        public string AuthToken { get; set; }
        public string FromPhoneNumber { get; set; }
    }
}