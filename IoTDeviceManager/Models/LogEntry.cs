using System;

namespace IoTDeviceManager.Models
{
    /// <summary>
    /// Represents a log entry for tracking application actions
    /// </summary>
    public class LogEntry
    {
        public DateTime Timestamp { get; set; }
        public string Action { get; set; }
        public string DeviceName { get; set; }
        public string Details { get; set; }
        public LogLevel Level { get; set; }

        public LogEntry(string action, string deviceName, string details, LogLevel level = LogLevel.Info)
        {
            Timestamp = DateTime.Now;
            Action = action;
            DeviceName = deviceName;
            Details = details;
            Level = level;
        }

        public override string ToString()
        {
            return $"[{Timestamp:yyyy-MM-dd HH:mm:ss}] [{Level}] {Action} - {DeviceName}: {Details}";
        }
    }

    public enum LogLevel
    {
        Info,
        Warning,
        Error,
        Success
    }
}