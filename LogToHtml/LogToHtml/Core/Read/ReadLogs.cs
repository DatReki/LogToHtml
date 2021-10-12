using System;
using HtmlAgilityPack;
using static LogToHtml.Logging;

namespace LogToHtml.Core.Read
{
	internal class ReadLogs
	{
		/// <summary>Get all of the logs that we wrote from the .html file</summary>
		internal static void ReadLogsFromFile(Options options)
		{
			HtmlDocument document = new();
			document.Load(options.FilePath);
			WriteLog.Html = document.DocumentNode.OuterHtml;
			foreach (string project in options.Projects)
			{
				//Get Logs for project
				HtmlNode projectDiv = document.DocumentNode.SelectSingleNode($"//div[@id='{project}']");

				// Get logs for each logType
				foreach (LogType typeOfLog in (LogType[])Enum.GetValues(typeof(LogType)))
				{
					//The "." at the start of the node selector is necessary to make it understand that it's an child elemment
					//In the below case logTypeDiv is selecting from the children of projectDiv
					HtmlNode logTypeDiv = projectDiv.SelectSingleNode($".//div[contains(@class, '{typeOfLog}')]");
					HtmlNodeCollection logs = logTypeDiv.SelectNodes($".//td");
					string date = null;
					for (int i = 0; i < logs.Count; ++i)
					{
						var item = logs[i];
						if (item.InnerText.Length > 0)
						{
							if (i % 2 == 0)
							{
								date = item.InnerText.Trim();
							}
							else
							{
								//Add the found log to our internal list of logs
								WriteLog.AddToListOfLogs(project, DateTime.Parse(date), typeOfLog, item.InnerText.Trim());
							}
						}
					}
				}
			}
		}
	}
}
