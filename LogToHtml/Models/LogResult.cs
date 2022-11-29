using System;

namespace LogToHtml.Models
{
	public class LogResult
	{
		public DateTime Date { get; set; }
		public Log.LogLevel LogLevel { get; set; }
		public string Message { get; set; } = string.Empty;
	}
}
