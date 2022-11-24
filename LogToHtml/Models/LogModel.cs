using System;
using static LogToHtml.Logging;
using System.Collections.Generic;

namespace LogToHtml.Models
{
    public class LogModel
    {
        public List<string> Projects { get; set; } = new();
        public string Project { get; set; } = string.Empty;
        public List<string> LogTypes { get; set; } = new();
        public LogLevel LogLevel { get; set; }
        public DateTime Date { get; set; }
        public string Error { get; set; } = string.Empty;
    }
}
