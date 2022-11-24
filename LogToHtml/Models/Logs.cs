using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogToHtml.Models
{
    public class Logs
    {
        public List<LogData> Critical { get; set; } = new();
        public List<LogData> Error { get; set; } = new();
        public List<LogData> Warn { get; set; } = new();
        public List<LogData> Info { get; set; } = new();
    }
}
