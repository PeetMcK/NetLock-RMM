using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Global.Helper;

namespace Windows.Helper;

public class Eventlog
{
    /// <summary>
    /// Event Log Entry Model for JSON serialization
    /// </summary>
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
    }

    /// <summary>
    /// Response model for event log queries
    /// </summary>
    public class EventLogResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("log_name")]
        public string LogName { get; set; }

        [JsonPropertyName("total_entries")]
        public int TotalEntries { get; set; }

        [JsonPropertyName("entries")]
        public List<EventLogEntry> Entries { get; set; }

        [JsonPropertyName("error")]
        public string Error { get; set; }

        [JsonPropertyName("timestamp")]
        public string Timestamp { get; set; }
    }

    /// <summary>
    /// Response model for available event logs list
    /// </summary>
    public class AvailableLogsResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("total_logs")]
        public int TotalLogs { get; set; }

        [JsonPropertyName("log_names")]
        public List<string> LogNames { get; set; }

        [JsonPropertyName("error")]
        public string Error { get; set; }

        [JsonPropertyName("timestamp")]
        public string Timestamp { get; set; }
    }

    /// <summary>
    /// Get available event log names on the system
    /// </summary>
    public static string GetAvailableEventLogs()
    {
        AvailableLogsResponse response = new AvailableLogsResponse
        {
            LogNames = new List<string>(),
            Timestamp = DateTime.UtcNow.ToString("o")
        };

        try
        {
            Logging.Debug("Windows.Helper.Eventlog", "GetAvailableEventLogs", "Getting available event logs");

            using (EventLogSession session = new EventLogSession())
            {
                var allLogNames = session.GetLogNames().OrderBy(name => name).ToList();
                var availableLogs = new List<string>();

                // Filter out disabled or unavailable logs
                foreach (var logName in allLogNames)
                {
                    try
                    {
                        EventLogConfiguration config = new EventLogConfiguration(logName, session);
                        
                        // Only include logs that are enabled and not empty
                        if (config.IsEnabled)
                        {
                            availableLogs.Add(logName);
                        }
                    }
                    catch (EventLogException)
                    {
                        // Skip logs that can't be accessed or configured
                        continue;
                    }
                    catch (UnauthorizedAccessException)
                    {
                        // Skip logs we don't have permission to access
                        continue;
                    }
                }

                response.LogNames = availableLogs;
                response.Success = true;
                response.TotalLogs = response.LogNames.Count;

                Logging.Debug("Windows.Helper.Eventlog", "GetAvailableEventLogs", 
                    $"Found {response.TotalLogs} available event logs (out of {allLogNames.Count} total)");
            }
        }
        catch (Exception ex)
        {
            Logging.Error("Windows.Helper.Eventlog", "GetAvailableEventLogs", ex.ToString());
            response.Success = false;
            response.Error = ex.Message;
        }

        return JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true });
    }

    /// <summary>
    /// Read event log entries from a specific log
    /// </summary>
    /// <param name="logName">Name of the event log (e.g., "Application", "System", "Security")</param>
    /// <param name="maxEntries">Maximum number of entries to return (default: 100, max: 10000)</param>
    /// <param name="level">Filter by level: 0=LogAlways, 1=Critical, 2=Error, 3=Warning, 4=Informational, 5=Verbose (null = all)</param>
    /// <param name="eventId">Filter by specific Event ID (null = all)</param>
    /// <param name="startTime">Filter events after this time (null = no filter)</param>
    /// <param name="endTime">Filter events before this time (null = no filter)</param>
    /// <param name="providerName">Filter by provider/source name (null = all)</param>
    /// <returns>JSON string with event log entries</returns>
    public static string ReadEventLog(
        string logName, 
        int maxEntries = 100, 
        byte? level = null, 
        int? eventId = null,
        DateTime? startTime = null,
        DateTime? endTime = null,
        string providerName = null)
    {
        EventLogResponse response = new EventLogResponse
        {
            LogName = logName,
            Entries = new List<EventLogEntry>(),
            Timestamp = DateTime.UtcNow.ToString("o")
        };

        try
        {
            // Validate maxEntries
            if (maxEntries < 1) maxEntries = 100;
            if (maxEntries > 10000) maxEntries = 10000;

            Logging.Debug("Windows.Helper.Eventlog", "ReadEventLog", 
                $"Reading log: {logName}, maxEntries: {maxEntries}, level: {level}, eventId: {eventId}");

            // Build XPath query
            string query = "*";
            List<string> conditions = new List<string>();

            if (level.HasValue)
                conditions.Add($"Level={level.Value}");

            if (eventId.HasValue)
                conditions.Add($"EventID={eventId.Value}");

            if (startTime.HasValue)
                conditions.Add($"TimeCreated[@SystemTime>='{startTime.Value.ToUniversalTime():o}']");

            if (endTime.HasValue)
                conditions.Add($"TimeCreated[@SystemTime<='{endTime.Value.ToUniversalTime():o}']");

            if (!string.IsNullOrWhiteSpace(providerName))
                conditions.Add($"@Name='{providerName}'");

            if (conditions.Count > 0)
                query = $"*[System[{string.Join(" and ", conditions)}]]";

            Logging.Debug("Windows.Helper.Eventlog", "ReadEventLog", $"XPath Query: {query}");

            // Create query
            EventLogQuery eventLogQuery = new EventLogQuery(logName, PathType.LogName, query)
            {
                ReverseDirection = true // Get newest entries first
            };

            EventLogReader reader = null;
            
            try
            {
                // Try to create the reader - this will fail if the log is disabled or not available
                reader = new EventLogReader(eventLogQuery);
            }
            catch (EventLogException ex)
            {
                // Check for specific error conditions
                if (ex.Message.Contains("not supported") || ex.Message.Contains("disabled"))
                {
                    response.Success = false;
                    response.Error = $"Event log '{logName}' is disabled or not available. Please enable it in Event Viewer or choose a different log.";
                    Logging.Error("Windows.Helper.Eventlog", "ReadEventLog", response.Error);
                    return JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true });
                }
                
                // Re-throw other EventLogExceptions to be caught by outer catch
                throw;
            }

            // Read events
            using (reader)
            {
                EventRecord eventRecord;
                long index = 0;

                while ((eventRecord = reader.ReadEvent()) != null && index < maxEntries)
                {
                    try
                    {
                        EventLogEntry entry = new EventLogEntry
                        {
                            Index = index++,
                            RecordId = eventRecord.RecordId,
                            LogName = eventRecord.LogName,
                            TimeCreated = eventRecord.TimeCreated?.ToUniversalTime().ToString("o") ?? "N/A",
                            EventId = eventRecord.Id,
                            Level = GetLevelName(eventRecord.Level),
                            LevelValue = eventRecord.Level,
                            ProviderName = eventRecord.ProviderName,
                            Source = eventRecord.ProviderName, // Source is same as ProviderName
                            Computer = eventRecord.MachineName,
                            User = eventRecord.UserId?.Value ?? "N/A",
                            TaskCategory = eventRecord.Task?.ToString() ?? "N/A",
                            TaskDisplayName = eventRecord.TaskDisplayName ?? "N/A",
                            Opcode = eventRecord.Opcode?.ToString() ?? "N/A",
                            OpcodeDisplayName = eventRecord.OpcodeDisplayName ?? "N/A",
                            ProcessId = eventRecord.ProcessId,
                            ThreadId = eventRecord.ThreadId,
                            ActivityId = eventRecord.ActivityId?.ToString() ?? null,
                            RelatedActivityId = eventRecord.RelatedActivityId?.ToString() ?? null,
                            Message = GetEventMessage(eventRecord),
                            Keywords = GetKeywords(eventRecord.Keywords),
                            KeywordsDisplayNames = GetKeywordsDisplayNames(eventRecord.KeywordsDisplayNames)
                        };

                        response.Entries.Add(entry);
                    }
                    catch (Exception entryEx)
                    {
                        Logging.Error("Windows.Helper.Eventlog", "ReadEventLog", 
                            $"Error reading event entry: {entryEx.Message}");
                    }
                    finally
                    {
                        eventRecord.Dispose();
                    }
                }
            }

            response.Success = true;
            response.TotalEntries = response.Entries.Count;

            Logging.Debug("Windows.Helper.Eventlog", "ReadEventLog", 
                $"Successfully read {response.TotalEntries} entries from {logName}");
        }
        catch (EventLogNotFoundException ex)
        {
            response.Success = false;
            response.Error = $"Event log '{logName}' not found: {ex.Message}";
            Logging.Error("Windows.Helper.Eventlog", "ReadEventLog", response.Error);
        }
        catch (UnauthorizedAccessException ex)
        {
            response.Success = false;
            response.Error = $"Access denied to event log '{logName}': {ex.Message}";
            Logging.Error("Windows.Helper.Eventlog", "ReadEventLog", response.Error);
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Error = $"Error reading event log: {ex.Message}";
            Logging.Error("Windows.Helper.Eventlog", "ReadEventLog", ex.ToString());
        }

        return JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true });
    }

    /// <summary>
    /// Get human-readable level name from level value
    /// </summary>
    private static string GetLevelName(byte? level)
    {
        if (!level.HasValue) return "Unknown";

        return level.Value switch
        {
            0 => "LogAlways",
            1 => "Critical",
            2 => "Error",
            3 => "Warning",
            4 => "Information",
            5 => "Verbose",
            _ => $"Unknown({level.Value})"
        };
    }

    /// <summary>
    /// Safely get the event message
    /// </summary>
    private static string GetEventMessage(EventRecord eventRecord)
    {
        try
        {
            return eventRecord.FormatDescription() ?? "No description available";
        }
        catch (EventLogException)
        {
            // Message template not found, try to construct from properties
            try
            {
                if (eventRecord.Properties != null && eventRecord.Properties.Count > 0)
                {
                    var properties = eventRecord.Properties
                        .Select(p => p.Value?.ToString() ?? "null")
                        .ToList();
                    return $"Event data: {string.Join(", ", properties)}";
                }
            }
            catch { }

            return "Description not available (message template missing)";
        }
        catch (Exception ex)
        {
            return $"Error retrieving message: {ex.Message}";
        }
    }

    /// <summary>
    /// Convert keywords bitmask to list of keyword values
    /// </summary>
    private static List<string> GetKeywords(long? keywords)
    {
        if (!keywords.HasValue || keywords.Value == 0)
            return new List<string>();

        List<string> keywordList = new List<string>();
        long value = keywords.Value;

        // Check each bit
        for (int i = 0; i < 64; i++)
        {
            long bit = 1L << i;
            if ((value & bit) != 0)
            {
                keywordList.Add($"0x{bit:X}");
            }
        }

        return keywordList;
    }

    /// <summary>
    /// Get keywords display names as list
    /// </summary>
    private static List<string> GetKeywordsDisplayNames(IEnumerable<string> keywordsDisplayNames)
    {
        if (keywordsDisplayNames == null)
            return new List<string>();

        return keywordsDisplayNames.Where(k => !string.IsNullOrWhiteSpace(k)).ToList();
    }

    /// <summary>
    /// Clear an event log (requires administrator privileges)
    /// </summary>
    /// <param name="logName">Name of the event log to clear</param>
    /// <returns>JSON string with result</returns>
    public static string ClearEventLog(string logName)
    {
        try
        {
            Logging.Debug("Windows.Helper.Eventlog", "ClearEventLog", $"Attempting to clear log: {logName}");

            using (EventLogSession session = new EventLogSession())
            {
                session.ClearLog(logName);

                var response = new
                {
                    success = true,
                    log_name = logName,
                    message = $"Event log '{logName}' cleared successfully",
                    timestamp = DateTime.UtcNow.ToString("o")
                };

                Logging.Debug("Windows.Helper.Eventlog", "ClearEventLog", $"Successfully cleared log: {logName}");
                return JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true });
            }
        }
        catch (UnauthorizedAccessException ex)
        {
            var errorResponse = new
            {
                success = false,
                log_name = logName,
                error = $"Access denied. Administrator privileges required: {ex.Message}",
                timestamp = DateTime.UtcNow.ToString("o")
            };
            Logging.Error("Windows.Helper.Eventlog", "ClearEventLog", errorResponse.error);
            return JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            var errorResponse = new
            {
                success = false,
                log_name = logName,
                error = ex.Message,
                timestamp = DateTime.UtcNow.ToString("o")
            };
            Logging.Error("Windows.Helper.Eventlog", "ClearEventLog", ex.ToString());
            return JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions { WriteIndented = true });
        }
    }

    /// <summary>
    /// Get statistics about an event log
    /// </summary>
    /// <param name="logName">Name of the event log</param>
    /// <returns>JSON string with log statistics</returns>
    public static string GetEventLogStats(string logName)
    {
        try
        {
            Logging.Debug("Windows.Helper.Eventlog", "GetEventLogStats", $"Getting stats for log: {logName}");

            using (EventLogSession session = new EventLogSession())
            {
                EventLogConfiguration logConfig = new EventLogConfiguration(logName, session);

                // Count entries by level
                Dictionary<string, int> levelCounts = new Dictionary<string, int>
                {
                    { "Critical", 0 },
                    { "Error", 0 },
                    { "Warning", 0 },
                    { "Information", 0 },
                    { "Verbose", 0 }
                };

                DateTime? oldestEntry = null;
                DateTime? newestEntry = null;
                long totalEntries = 0;

                // Read through events to gather statistics
                EventLogQuery query = new EventLogQuery(logName, PathType.LogName, "*");
                using (EventLogReader reader = new EventLogReader(query))
                {
                    EventRecord eventRecord;
                    while ((eventRecord = reader.ReadEvent()) != null)
                    {
                        totalEntries++;

                        if (eventRecord.TimeCreated.HasValue)
                        {
                            if (!oldestEntry.HasValue || eventRecord.TimeCreated.Value < oldestEntry.Value)
                                oldestEntry = eventRecord.TimeCreated.Value;

                            if (!newestEntry.HasValue || eventRecord.TimeCreated.Value > newestEntry.Value)
                                newestEntry = eventRecord.TimeCreated.Value;
                        }

                        string levelName = GetLevelName(eventRecord.Level);
                        if (levelCounts.ContainsKey(levelName))
                            levelCounts[levelName]++;

                        eventRecord.Dispose();
                    }
                }

                var response = new
                {
                    success = true,
                    log_name = logName,
                    is_enabled = logConfig.IsEnabled,
                    log_type = logConfig.LogType.ToString(),
                    log_mode = logConfig.LogMode.ToString(),
                    maximum_size_bytes = logConfig.MaximumSizeInBytes,
                    log_file_path = logConfig.LogFilePath,
                    total_entries = totalEntries,
                    oldest_entry = oldestEntry?.ToUniversalTime().ToString("o"),
                    newest_entry = newestEntry?.ToUniversalTime().ToString("o"),
                    level_counts = levelCounts,
                    timestamp = DateTime.UtcNow.ToString("o")
                };

                Logging.Debug("Windows.Helper.Eventlog", "GetEventLogStats", 
                    $"Stats for {logName}: {totalEntries} entries");
                return JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true });
            }
        }
        catch (Exception ex)
        {
            var errorResponse = new
            {
                success = false,
                log_name = logName,
                error = ex.Message,
                timestamp = DateTime.UtcNow.ToString("o")
            };
            Logging.Error("Windows.Helper.Eventlog", "GetEventLogStats", ex.ToString());
            return JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions { WriteIndented = true });
        }
    }
}