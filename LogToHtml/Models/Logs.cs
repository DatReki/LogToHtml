using System.Collections.Generic;

namespace LogToHtml.Models
{
	public class GetLogs
	{
		public List<LogData> Critical { get; set; } = new();
		public List<LogData> Error { get; set; } = new();
		public List<LogData> Warn { get; set; } = new();
		public List<LogData> Info { get; set; } = new();
	}
}
