using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace QuestGuildTerminal
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("🏰 Welcome to the Quest Guild Terminal! 🏰");
            Console.WriteLine("==========================================\n");
            
            try
            {
                // Initialize database
                await DatabaseInitializer.InitializeDatabaseAsync();
                
                // Test notifications
                await TestNotificationServices();
                
                Console.WriteLine("\nPress any key to start the application...");
                Console.ReadKey();
                
                // Start the app with database services
                var app = new QuestGuildApp();
                app.Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Application failed to start: {ex.Message}");
                Console.WriteLine("🚨 Press any key to exit...");
                Console.ReadKey();
                Environment.Exit(1);
            }
        }

        static async Task TestNotificationServices()
        {
            Console.WriteLine("🧪 Testing notification services...");
            
            var notificationConfig = AppConfig.GetNotificationConfig();
            var notificationService = new EnhancedNotificationService(notificationConfig);
            
            // Test email - use the correct method name
            if (notificationConfig.IsEmailEnabled)
            {
                Console.WriteLine("📧 Testing email service...");
                await notificationService.Send2FACodeNotificationAsync( // FIXED: Correct method name
                    "test@example.com", 
                    "123456", 
                    "TestHero"
                );
            }
            
            // Test SMS
            if (notificationConfig.IsSmsEnabled)
            {
                Console.WriteLine("📱 Testing SMS service...");
                await notificationService.SendNotificationAsync(
                    "Test message from Quest Guild", 
                    "+1234567890"  // Use your phone number for testing
                );
            }
            
            if (!notificationConfig.IsEmailEnabled && !notificationConfig.IsSmsEnabled)
            {
                Console.WriteLine("ℹ️ Real notifications not configured - using simulated services");
            }
        }
        // ADD THIS METHOD: Complete API key tester
        public static async Task TestApiKey(string apiKey)
        {
            try
            {
                Console.WriteLine("🧪 Testing Gemini API Key...");
                Console.WriteLine($"🔑 API Key: {apiKey?.Substring(0, Math.Min(10, apiKey.Length))}...");
                
                using var httpClient = new HttpClient();
                
                // Try multiple endpoints
                var endpoints = new[]
                {
                    "https://generativelanguage.googleapis.com/v1/models/gemini-1.5-flash:generateContent",
                    "https://generativelanguage.googleapis.com/v1/models/gemini-1.5-pro:generateContent", 
                    "https://generativelanguage.googleapis.com/v1/models/gemini-pro:generateContent",
                    "https://generativelanguage.googleapis.com/v1beta/models/gemini-pro:generateContent"
                };
                
                bool anyWorking = false;
                
                foreach (var endpoint in endpoints)
                {
                    try
                    {
                        var url = $"{endpoint}?key={apiKey}";
                        var requestBody = @"{
                            ""contents"": [{
                                ""parts"": [{
                                    ""text"": ""Say 'Hello World' in a heroic way""
                                }]
                            }]
                        }";
                        
                        var content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                        var response = await httpClient.PostAsync(url, content);
                        
                        Console.WriteLine($"🔍 {endpoint.Split('/').Last()} → {response.StatusCode}");
                        
                        if (response.IsSuccessStatusCode)
                        {
                            anyWorking = true;
                            var responseText = await response.Content.ReadAsStringAsync();
                            Console.WriteLine($"✅ SUCCESS with {endpoint.Split('/').Last()}!");
                            
                            // Try to extract the response text
                            try
                            {
                                var startIndex = responseText.IndexOf("\"text\": \"") + 9;
                                if (startIndex >= 9)
                                {
                                    var endIndex = responseText.IndexOf("\"", startIndex);
                                    if (endIndex > startIndex)
                                    {
                                        var aiResponse = responseText.Substring(startIndex, endIndex - startIndex);
                                        Console.WriteLine($"🤖 AI Response: {aiResponse}");
                                    }
                                }
                            }
                            catch (Exception parseEx)
                            {
                                Console.WriteLine($"⚠️ Could not parse response: {parseEx.Message}");
                            }
                            break; // Stop at first working endpoint
                        }
                        else
                        {
                            var errorContent = await response.Content.ReadAsStringAsync();
                            Console.WriteLine($"   Error: {errorContent.Substring(0, Math.Min(100, errorContent.Length))}...");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"❌ {endpoint.Split('/').Last()} → Exception: {ex.Message}");
                    }
                    
                    await Task.Delay(500); // Small delay between attempts
                }
                
                if (!anyWorking)
                {
                    Console.WriteLine("\n❌ No working API endpoints found.");
                    Console.WriteLine("🔧 Using enhanced simulated AI instead.");
                    Console.WriteLine("💡 To enable real AI:");
                    Console.WriteLine("   1. Go to Google Cloud Console");
                    Console.WriteLine("   2. Enable 'Generative Language API'");
                    Console.WriteLine("   3. Ensure billing is set up");
                    Console.WriteLine("   4. Check API key restrictions");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"💥 API Test Failed: {ex.Message}");
            }
        }

        // ADD THIS METHOD: List available models
        public static async Task ListAvailableModels(string apiKey)
        {
            try
            {
                Console.WriteLine("\n🔍 Checking available models...");

                using var httpClient = new HttpClient();
                var listUrl = $"https://generativelanguage.googleapis.com/v1/models?key={apiKey}";
                var response = await httpClient.GetAsync(listUrl);

                Console.WriteLine($"📡 Model List Status: {response.StatusCode}");

                if (response.IsSuccessStatusCode)
                {
                    var responseText = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("✅ Available Models Response:");
                    Console.WriteLine(responseText.Length > 500 ? responseText.Substring(0, 500) + "..." : responseText);
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"❌ Cannot list models: {errorContent}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"💥 Model List Failed: {ex.Message}");
            }
        }
        
        
    }
}