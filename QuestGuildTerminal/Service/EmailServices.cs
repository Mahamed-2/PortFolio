// Services/EmailServices.cs
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace QuestGuildTerminal
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(string toEmail, string subject, string htmlContent, string plainTextContent = null);
    }

    public interface ISmsService
    {
        Task<bool> SendSmsAsync(string toPhoneNumber, string message);
    }

    // Real Email Service using SendGrid
    public class RealEmailService : IEmailService
    {
        private readonly EmailConfig _config;
        private readonly HttpClient _httpClient;

        public RealEmailService(EmailConfig config)
        {
            _config = config;
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("Bearer", _config.ApiKey);
        }

        public async Task<bool> SendEmailAsync(string toEmail, string subject, string htmlContent, string plainTextContent = null)
        {
            try
            {
                var requestBody = new
                {
                    personalizations = new[]
                    {
                        new
                        {
                            to = new[] { new { email = toEmail } },
                            subject = subject
                        }
                    },
                    from = new { email = _config.FromEmail, name = _config.FromName },
                    content = new[]
                    {
                        new { type = "text/plain", value = plainTextContent ?? htmlContent },
                        new { type = "text/html", value = htmlContent }
                    }
                };

                var json = System.Text.Json.JsonSerializer.Serialize(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(
                    "https://api.sendgrid.com/v3/mail/send", content);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"✅ Email sent successfully to {toEmail}");
                    return true;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"❌ Email failed to {toEmail}: {response.StatusCode} - {errorContent}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"💥 Email exception to {toEmail}: {ex.Message}");
                return false;
            }
        }
    }

    // Real SMS Service using Twilio
    public class RealSmsService : ISmsService
    {
        private readonly SmsConfig _config;
        private readonly HttpClient _httpClient;

        public RealSmsService(SmsConfig config)
        {
            _config = config;
            _httpClient = new HttpClient();
            
            // Basic authentication for Twilio
            var byteArray = Encoding.ASCII.GetBytes($"{_config.AccountSid}:{_config.AuthToken}");
            _httpClient.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
        }

        public async Task<bool> SendSmsAsync(string toPhoneNumber, string message)
        {
            try
            {
                // Format phone number if needed
                var formattedTo = FormatPhoneNumber(toPhoneNumber);
                var formattedFrom = FormatPhoneNumber(_config.FromPhoneNumber);

                var formData = new System.Collections.Generic.Dictionary<string, string>
                {
                    ["To"] = formattedTo,
                    ["From"] = formattedFrom,
                    ["Body"] = message
                };

                var content = new FormUrlEncodedContent(formData);

                var response = await _httpClient.PostAsync(
                    $"https://api.twilio.com/2010-04-01/Accounts/{_config.AccountSid}/Messages.json", content);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"✅ SMS sent successfully to {formattedTo}");
                    return true;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"❌ SMS failed to {formattedTo}: {response.StatusCode} - {errorContent}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"💥 SMS exception to {toPhoneNumber}: {ex.Message}");
                return false;
            }
        }

        private string FormatPhoneNumber(string phoneNumber)
        {
            // Remove any non-digit characters and ensure proper formatting
            var digits = System.Text.RegularExpressions.Regex.Replace(phoneNumber, @"[^\d]", "");
            
            // If it's 10 digits, assume US number and add +1
            if (digits.Length == 10)
                return $"+1{digits}";
            
            // If it starts with 1 but has 11 digits, add +
            if (digits.Length == 11 && digits.StartsWith("1"))
                return $"+{digits}";
            
            // If it already has country code, ensure it starts with +
            if (digits.Length >= 10 && !digits.StartsWith("+"))
                return $"+{digits}";
            
            return phoneNumber; // Return as-is if already formatted
        }
    }

    // Simulated services for when real services aren't configured
    public class SimulatedEmailService : IEmailService
    {
        public async Task<bool> SendEmailAsync(string toEmail, string subject, string htmlContent, string plainTextContent = null)
        {
            Console.WriteLine($"📧 [SIMULATED] Email to {toEmail}");
            Console.WriteLine($"   Subject: {subject}");
            Console.WriteLine($"   Message: {plainTextContent ?? htmlContent}");
            await Task.Delay(500); // Simulate sending time
            return true;
        }
    }

    public class SimulatedSmsService : ISmsService
    {
        public async Task<bool> SendSmsAsync(string toPhoneNumber, string message)
        {
            Console.WriteLine($"📱 [SIMULATED] SMS to {toPhoneNumber}");
            Console.WriteLine($"   Message: {message}");
            await Task.Delay(300); // Simulate sending time
            return true;
        }
    }
}