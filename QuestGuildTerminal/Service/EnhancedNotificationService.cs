// Services/EnhancedNotificationService.cs - Also update this one
using System;
using System.Linq;
using System.Threading.Tasks;

namespace QuestGuildTerminal
{
    public class EnhancedNotificationService : INotificationService
    {
        private readonly IEmailService _emailService;
        private readonly ISmsService _smsService;
        private readonly NotificationConfig _config;

        public EnhancedNotificationService(NotificationConfig config)
        {
            _config = config;
            
            // Initialize real services if configured, otherwise use simulated
            _emailService = _config.IsEmailEnabled 
                ? new RealEmailService(config.Email) //? means if true use this
                : new SimulatedEmailService();//: if false use this
                
            _smsService = _config.IsSmsEnabled 
                ? new RealSmsService(config.Sms) 
                : new SimulatedSmsService();

            Console.WriteLine($"📧 Email Service: {(_config.IsEmailEnabled ? "REAL (SendGrid)" : "SIMULATED")}");
            Console.WriteLine($"📱 SMS Service: {(_config.IsSmsEnabled ? "REAL (Twilio)" : "SIMULATED")}");
        }

        public async Task SendNotificationAsync(string message, string contactInfo)
        {
            // Determine if contactInfo is email or phone number
            if (IsEmail(contactInfo))
            {
                await SendEmailNotificationAsync(contactInfo, message);
            }
            else if (IsPhoneNumber(contactInfo))
            {
                await SendSmsNotificationAsync(contactInfo, message);
            }
            else
            {
                Console.WriteLine($"⚠️ Unknown contact type: {contactInfo}");
            }
        }

        // CHANGED: Accept IQuestManager instead of QuestManager
        public async Task CheckDeadlineNotificationsAsync(IQuestManager questManager, string heroContact)
        {
            var nearDeadlineQuests = questManager.GetQuestsNearDeadline();
            
            foreach (var quest in nearDeadlineQuests)
            {
                var message = $"⚔️ URGENT: Hero, your quest '{quest.Title}' must be completed by {quest.DueDate:MMMM dd}! The guild is counting on you!";
                await SendNotificationAsync(message, heroContact);
            }

            if (!nearDeadlineQuests.Any())
            {
                Console.WriteLine("✅ No urgent deadline notifications at this time.");
            }
        }

        public async Task Send2FACodeNotificationAsync(string contactInfo, string code, string heroName)
        {
            var emailMessage = $@"
<html>
<body>
    <h2>🏰 Quest Guild Terminal - Verification Code</h2>
    <p>Hello {heroName},</p>
    <p>Your verification code is: <strong style='font-size: 24px; color: #2E86AB;'>{code}</strong></p>
    <p>Enter this code in the Quest Guild Terminal to continue your adventure.</p>
    <p><em>This code will expire in 10 minutes.</em></p>
    <hr>
    <p style='color: #666;'>If you didn't request this code, please ignore this message.</p>
</body>
</html>";

            var smsMessage = $"🏰 Quest Guild: Hello {heroName}, your verification code is {code}. It expires in 10 minutes.";

            if (IsEmail(contactInfo))
            {
                await _emailService.SendEmailAsync(
                    contactInfo, 
                    "Your Quest Guild Verification Code", 
                    emailMessage,
                    $"Hello {heroName}, your verification code is {code}. It expires in 10 minutes."
                );
            }
            else if (IsPhoneNumber(contactInfo))
            {
                await _smsService.SendSmsAsync(contactInfo, smsMessage);
            }
        }

        private async Task SendEmailNotificationAsync(string email, string message)
        {
            var htmlMessage = $@"
<html>
<body>
    <h2>🏰 Quest Guild Alert</h2>
    <p>{message}</p>
    <hr>
    <p style='color: #666;'>Safe travels, hero!</p>
</body>
</html>";

            await _emailService.SendEmailAsync(
                email,
                "Quest Guild Notification",
                htmlMessage,
                message
            );
        }

        private async Task SendSmsNotificationAsync(string phoneNumber, string message)
        {
            await _smsService.SendSmsAsync(phoneNumber, $"🏰 Quest Guild: {message}");
        }

        private bool IsEmail(string contactInfo)
        {
            return contactInfo.Contains("@") && contactInfo.Contains(".");
        }

        private bool IsPhoneNumber(string contactInfo)
        {
            var digits = System.Text.RegularExpressions.Regex.Replace(contactInfo, @"[^\d]", "");
            return digits.Length >= 10;
        }
    }
}