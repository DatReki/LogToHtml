using System;

namespace LogToHtml.Models
{
    public class LogResult
    {
        public DateTime Date { get; set; }
        public Logging.LogType LogType { get; set; }
        public string Message { get; set; }
    }
}
