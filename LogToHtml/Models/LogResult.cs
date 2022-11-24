using System;

namespace LogToHtml.Models
{
    public class LogResult
    {
        public DateTime Date { get; set; }
        public Logging.LogLevel LogLevel { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
