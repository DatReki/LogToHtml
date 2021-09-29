﻿using System;
using HtmlAgilityPack;
using static LogToHtml.Logging;

namespace LogToHtml.Core.Edit
{
	internal class EditExistingLog
	{
		/// <summary>Updates the .html file to add a new log</summary>
		internal static string Edit(Options options, LogType logType, string error)
		{
			HtmlDocument document = new();
			document.Load(options.FilePath);
			HtmlNode table = document.DocumentNode.SelectSingleNode($"//div[@id='{options.Project}-{logType}-textBox']/table");
			table.AppendChild(HtmlNode.CreateNode($@"
				<tr>
					<td>{DateTime.UtcNow}</td>
					<td>{error}</td>
				</tr>"));
			return document.DocumentNode.OuterHtml;
		}
	}
}
