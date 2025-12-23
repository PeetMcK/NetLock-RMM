using System.Text.Json.Serialization;

namespace NetLock_RMM_Web_Console.Components.Pages.Devices.Dialogs.Remote_EventLog
{
    public class EventLogStatsResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("log_name")]
        public string LogName { get; set; }

        [JsonPropertyName("is_enabled")]
        public bool IsEnabled { get; set; }

        [JsonPropertyName("log_type")]
        public string LogType { get; set; }

        [JsonPropertyName("log_mode")]
        public string LogMode { get; set; }

        [JsonPropertyName("maximum_size_bytes")]
        public long MaximumSizeBytes { get; set; }

        [JsonPropertyName("log_file_path")]
        public string LogFilePath { get; set; }

        [JsonPropertyName("total_entries")]
        public long TotalEntries { get; set; }

        [JsonPropertyName("oldest_entry")]
        public string OldestEntry { get; set; }

        [JsonPropertyName("newest_entry")]
        public string NewestEntry { get; set; }

        [JsonPropertyName("level_counts")]
        public Dictionary<string, int> LevelCounts { get; set; }

        [JsonPropertyName("error")]
        public string Error { get; set; }

        [JsonPropertyName("timestamp")]
        public string Timestamp { get; set; }
    }
}

