using System;
using static LogToHtml.Logging;
using System.Collections.Generic;

namespace LogToHtml.Core
{
	internal class WriteLog
	{
		internal static bool ReadFromFile = true;

		/// <summary>Creates the .html file from an embedded .cshtml file. This will be used as our log file</summary>
		internal static void CreateLog(Options options, LogType logType, string error)
		{
			string html = Create.Render.RenderViewAsync(options, logType, error).Result;
			ReadFromFile = false;
			AddToListOfLogs(options.Project, options.Date, logType, error);
			html.FormatHtml().WriteToFile(options.FilePath);
		}

		/// <summary>Edits the .html file containing all of our logs</summary>
		internal static void EditLog(Options options, LogType logType, string error)
		{
			if (ReadFromFile)
			{
				Read.ReadLogs.ReadLogsFromFile(options);
				ReadFromFile = false;
			}
			else
			{
				AddToListOfLogs(options.Project, options.Date, logType, error);
			}

			string html = Edit.EditExistingLog.Edit(options, logType, error);
			html.FormatHtml().WriteToFile(options.FilePath);
		}

		/// <summary>Updates our lists containing all of the errors</summary>
		internal static void AddToListOfLogs(string project, DateTime date, LogType logType, string error)
		{
			switch (logType)
			{
				case LogType.Critical:
					AllLogs.Critical.Add(new AllLogs.LogVariables() { Project = project, Date = date, Error = error });
					break;
				case LogType.Error:
					AllLogs.Error.Add(new AllLogs.LogVariables() { Project = project, Date = date, Error = error });
					break;
				case LogType.Warn:
					AllLogs.Warn.Add(new AllLogs.LogVariables() { Project = project, Date = date, Error = error });
					break;
				case LogType.Info:
					AllLogs.Info.Add(new AllLogs.LogVariables() { Project = project, Date = date, Error = error });
					break;
			}
		}
	}

	public class AllLogs
	{
		public static readonly List<LogVariables> Info = new();
		public static readonly List<LogVariables> Warn = new();
		public static readonly List<LogVariables> Error = new();
		public static readonly List<LogVariables> Critical = new();

		public partial class LogVariables
		{
			public string Project { get; set; }
			public DateTime Date { get; set; }
			public string Error { get; set; }
		}
	}
}
