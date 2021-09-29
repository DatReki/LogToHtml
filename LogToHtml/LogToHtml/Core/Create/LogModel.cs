using System;
using static LogToHtml.Logging;
using System.Collections.Generic;

namespace LogToHtml.Core.Create
{
    public class LogModel
    {
        public List<string> Projects { get; set; }
        public string Project { get; set; }
        public List<string> LogTypes { get; set; }
        public LogType LogType { get; set; }
        public DateTime Date { get; set; }
        public string Error { get; set; }
    }
}
