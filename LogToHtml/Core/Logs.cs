﻿using HtmlAgilityPack;
using LogToHtml.Models;
using RazorLight;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static LogToHtml.Core.Data;
using static LogToHtml.Log;

namespace LogToHtml.Core
{
	internal class Logs
	{
		/// <summary>
		/// Indicate if the library needs to read from a existing log file.
		/// </summary>
		private static bool _readLogFile = true;

		/// <summary>
		/// A string containing the HTML from the log file.
		/// </summary>
		private static string _html = string.Empty;

		/// <summary>
		/// Render the .cshtml file to a string.
		/// </summary>
		internal class Render
		{
			private static readonly RazorLightEngine _engine = new RazorLightEngineBuilder()
				.UseEmbeddedResourcesProject(typeof(Render))
				.SetOperatingAssembly(typeof(Render).Assembly)
				.UseMemoryCachingProvider()
				.Build();

			/// <summary>
			/// Use RazorLight to get the embedded .cshtml file and convert it to a string.
			/// </summary>
			internal static async Task<string> RenderViewAsync()
			{
				LogModel model = new()
				{
					Projects = Configuration.Projects,
					LogLevels = new List<string>()
					{
						LogLevel.Critical.ToString().ToLower(),
						LogLevel.Error.ToString().ToLower(),
						LogLevel.Warn.ToString().ToLower(),
						LogLevel.Info.ToString().ToLower()
					},
				};

				return await _engine.CompileRenderAsync("Views.MainPage", model);
			}
		}

		/// <summary>
		/// Creates the .html file from an embedded .cshtml file. This will be used as our log file.
		/// </summary>
		/// <param name="options">Options for the log entry.</param>
		/// <param name="logType">The logs LogType.</param>
		/// <param name="message">The message associated with the log.</param>
		internal static async void Create(Options options, LogLevel logType, string message)
		{
			// Create & format a HTML file from the embedded .cshtml file
			_html = (await Render.RenderViewAsync()).FormatHtml();
			// Add first log entry to the log file
			string logHtml = AddLogEntry(options, logType, message).FormatHtml();
			// Indicate we don't need to read from the log file as we just created a new one
			_readLogFile = false;
			// Add log to list of all logs
			AddToListOfLogs(options.Project, Configuration.Date, logType, message);
			// Update static HTML string
			_html = logHtml;
			// Add new entry to log file
			logHtml.WriteToFile();
		}

		/// <summary>
		/// Edits the .html file containing all of our logs.
		/// </summary>
		/// <param name="options">Options for the log entry.</param>
		/// <param name="logType">The logs LogType.</param>
		/// <param name="message">The message associated with the log.</param>
		internal static void Edit(Options options, LogLevel logType, string message)
		{
			if (_readLogFile)
			{
				ReadLogsFromFile();
				_readLogFile = false;
			}
			else
				AddToListOfLogs(options.Project, Configuration.Date, logType, message);

			// Add log to HTML string
			string html = AddLogEntry(options, logType, message);
			// Format HTML string
			html = html.FormatHtml();
			// Update static HTML string
			_html = html;
			// Write updated HTML string to log file
			html.WriteToFile();
		}

		/// <summary>
		/// Get all log entries from the .html file.
		/// And pass the HTML from the file to the 'Html' variable.
		/// </summary>
		private static void ReadLogsFromFile()
		{
			HtmlDocument document = new();
			document.Load(Configuration.LogFile.Full);
			_html = document.DocumentNode.OuterHtml;
			foreach (string project in Configuration.Projects)
			{
				// Get Logs for project
				HtmlNode projectDiv = document.DocumentNode.SelectSingleNode($"//div[@id='{project}']");

				// Get logs for each logType
				foreach (LogLevel typeOfLog in (LogLevel[])Enum.GetValues(typeof(LogLevel)))
				{
					// The "." at the start of the node selector is necessary to make it understand that it's an child elemment
					// In the below case logTypeDiv is selecting from the children of projectDiv
					HtmlNode logTypeDiv = projectDiv.SelectSingleNode($".//div[contains(@class, '{typeOfLog.ToString().ToLower()}')]");
					HtmlNodeCollection logs = logTypeDiv.SelectNodes($".//td");
					string date = string.Empty;
					if (logs != null)
					{
						for (int i = 0; i < logs.Count; ++i)
						{
							HtmlNode item = logs[i];
							if (item.InnerText.Length > 0)
							{
								if (i % 2 == 0)
									date = item.InnerText.Trim();
								else
									AddToListOfLogs(project, DateTime.Parse(date), typeOfLog, item.InnerText.Trim());
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Updates the lists containing all log entries.
		/// </summary>
		/// <param name="project">Name of the project the log was written from.</param>
		/// <param name="date">Date the log was written.</param>
		/// <param name="logLevel">The logs LogType.</param>
		/// <param name="message">The message associated with the log.</param>
		internal static void AddToListOfLogs(string project, DateTime date, LogLevel logLevel, string message)
		{
			switch (logLevel)
			{
				case LogLevel.Critical:
					AllLogs.Critical.Add(new LogData() { Project = project, Date = date, Error = message });
					break;
				case LogLevel.Error:
					AllLogs.Error.Add(new LogData() { Project = project, Date = date, Error = message });
					break;
				case LogLevel.Warn:
					AllLogs.Warn.Add(new LogData() { Project = project, Date = date, Error = message });
					break;
				case LogLevel.Info:
					AllLogs.Info.Add(new LogData() { Project = project, Date = date, Error = message });
					break;
			}
		}

		/// <summary>
		/// Updates the .html file to add a new entry.
		/// </summary>
		/// <param name="options">Options for the new entry.</param>
		/// <param name="logLevel">LogType for the new entry.</param>
		/// <param name="message">Message for the new entry.</param>
		/// <returns></returns>
		private static string AddLogEntry(Options options, LogLevel logLevel, string message)
		{
			HtmlDocument document = new();
			// If 'Html' is empty load from file.
			// Otherwise load from string.
			if (string.IsNullOrEmpty(_html))
			{
				document.Load(Configuration.LogFile.Full);
				_html = document.DocumentNode.OuterHtml;
			}
			else
				document.LoadHtml(_html);

			HtmlNode table = document.DocumentNode.SelectSingleNode($"//div[@id='{options.Project}-{logLevel.ToString().ToLower()}-textBox']/table/tbody");
			table.AppendChild(HtmlNode.CreateNode($@"
				<tr>
					<td class=""dates"">
                        <p>
                            {DateTime.Now}
                        </p>
                    </td>
					<td class=""messages"">
                        <p>
                            {message.Replace("\r\n", "<br>").Replace("\n", "<br>").Replace("\r", "<br>")}
                        </p>
                    </td>
				</tr>"));
			return document.DocumentNode.OuterHtml;
		}
	}
}
