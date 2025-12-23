using System.Text.Json.Serialization;

namespace NetLock_RMM_Web_Console.Components.Pages.Devices.Dialogs.Remote_EventLog
{
    public class EventLogEntry
    {
        [JsonPropertyName("index")]
        public long Index { get; set; }

        [JsonPropertyName("time_created")]
        public string TimeCreated { get; set; }

        [JsonPropertyName("event_id")]
        public int? EventId { get; set; }

        [JsonPropertyName("level")]
        public string Level { get; set; }

        [JsonPropertyName("level_value")]
        public byte? LevelValue { get; set; }

        [JsonPropertyName("provider_name")]
        public string ProviderName { get; set; }

        [JsonPropertyName("source")]
        public string Source { get; set; }

        [JsonPropertyName("computer")]
        public string Computer { get; set; }

        [JsonPropertyName("user")]
        public string User { get; set; }

        [JsonPropertyName("task_category")]
        public string TaskCategory { get; set; }

        [JsonPropertyName("task_display_name")]
        public string TaskDisplayName { get; set; }

        [JsonPropertyName("opcode")]
        public string Opcode { get; set; }

        [JsonPropertyName("opcode_display_name")]
        public string OpcodeDisplayName { get; set; }

        [JsonPropertyName("keywords")]
        public List<string> Keywords { get; set; }

        [JsonPropertyName("keywords_display_names")]
        public List<string> KeywordsDisplayNames { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("process_id")]
        public int? ProcessId { get; set; }

        [JsonPropertyName("thread_id")]
        public int? ThreadId { get; set; }

        [JsonPropertyName("activity_id")]
        public string ActivityId { get; set; }

        [JsonPropertyName("related_activity_id")]
        public string RelatedActivityId { get; set; }

        [JsonPropertyName("record_id")]
        public long? RecordId { get; set; }

        [JsonPropertyName("log_name")]
        public string LogName { get; set; }

        /// <summary>
        /// Get formatted time created string
        /// </summary>
        public string GetFormattedTimeCreated()
        {
            if (string.IsNullOrEmpty(TimeCreated))
                return "N/A";

            try
            {
                if (DateTime.TryParse(TimeCreated, out DateTime dt))
                {
                    return dt.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss");
                }
                return TimeCreated;
            }
            catch
            {
                return TimeCreated;
            }
        }
    }
}

