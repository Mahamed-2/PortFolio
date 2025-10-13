namespace QuestGuildTerminal
{
    public class GeminiConfig
    {
        public string ApiKey { get; set; }
        public string Model { get; set; } = "gemini-pro";
        public double Temperature { get; set; } = 0.7;
        public int MaxRetries { get; set; } = 3;
        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);
        
        // M2: "Configuration class allows easy extension without breaking changes"
        public GeminiConfig(string apiKey)
        {
            ApiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
        }
    }
}