using System;

namespace LogToHtml.Models
{
    public class LogData
    {
        public string Project { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Error { get; set; } = string.Empty;
    }
}
