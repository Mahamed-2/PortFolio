// AppConfig.cs


namespace QuestGuildTerminal
{
    public static class AppConfig
    {
        public static string GetGeminiApiKey()
        {
            //My existing API key logic.
            return null; // using simulated AI for now
        }
        public static NotificationConfig GetNotificationConfig()
        {
            return new NotificationConfig
            {
                Email = new EmailConfig
                {
                    // Get from environment variables or app settings
                    ApiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY"),
                    FromEmail = Environment.GetEnvironmentVariable("SENDGRID_FROM_EMAIL") ?? "questguild@yourdomain.com",//default email if env var is not set and ?? means only if null use this
                    FromName = "Quest Guild Terminal"
                },
                Sms = new SmsConfig
                {
                    // Get from environment variables or app settings
                    AccountSid = Environment.GetEnvironmentVariable("TWILIO_ACCOUNT_SID"),
                    AuthToken = Environment.GetEnvironmentVariable("TWILIO_AUTH_TOKEN"),
                    FromPhoneNumber = Environment.GetEnvironmentVariable("TWILIO_PHONE_NUMBER")
                }
            };
        }

        public static bool AreRealNotificationsEnabled()
        {
            var config = GetNotificationConfig();
            return config.IsEmailEnabled || config.IsSmsEnabled;
        }
    }
}