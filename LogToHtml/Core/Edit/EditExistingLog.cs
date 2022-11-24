using System;
using HtmlAgilityPack;
using static LogToHtml.Logging;

namespace LogToHtml.Core.Edit
{
	internal class EditExistingLog
	{
		/// <summary>
		/// Updates the .html file to add a new entry.
		/// </summary>
		/// <param name="options">Options for the new entry.</param>
		/// <param name="logType">LogType for the new entry.</param>
		/// <param name="message">Message for the new entry.</param>
		/// <returns></returns>
		internal static string Edit(Options options, LogLevel logType, string message)
		{
			HtmlDocument document = new();
			if (string.IsNullOrEmpty(WriteLog.Html))
			{
				document.Load(Configuration.LogFile.Full);
				WriteLog.Html = document.DocumentNode.OuterHtml;
			}
			else
				document.LoadHtml(WriteLog.Html);

			HtmlNode table = document.DocumentNode.SelectSingleNode($"//div[@id='{options.Project}-{logType}-textBox']/table");
			table.AppendChild(HtmlNode.CreateNode($@"
				<tr>
					<td>{DateTime.Now}</td>
					<td>{message}</td>
				</tr>"));
			return document.DocumentNode.OuterHtml;
		}
	}
}
