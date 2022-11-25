using LogToHtml.Core;
using LogToHtml.Models;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using static LogToHtml.Core.Data;

namespace LogToHtml
{
	public class Log
	{
		public class Options
		{
			public string Project { get; set; } = string.Empty;
			public bool LogToConsole = false;
		}

		public enum LogLevel
		{
			Info,
			Warn,
			Error,
			Critical,
		}

		/// <summary>
		/// Write a information log.
		/// </summary>
		/// <param name="options">The options for this log entry.</param>
		/// <param name="message">The message/exception you want to log.</param>
		public static LogResult Info(Options options, string message,
			[CallerMemberName] string method = "",
			[CallerFilePath] string file = "",
			[CallerLineNumber] int lineNumber = 0) => Base(options, LogLevel.Info, message, file, method, lineNumber);

		/// <summary>
		/// Write a warning log.
		/// </summary>
		/// <param name="options">The options for this log entry.</param>
		/// <param name="message">The message/exception you want to log.</param>
		public static LogResult Warn(Options options, string message,
			[CallerMemberName] string method = "",
			[CallerFilePath] string file = "",
			[CallerLineNumber] int lineNumber = 0) => Base(options, LogLevel.Warn, message, file, method, lineNumber);

		/// <summary>
		/// Write a error log.
		/// </summary>
		/// <param name="options">The options for this log entry.</param>
		/// <param name="message">The message/exception you want to log.</param>
		public static LogResult Error(Options options, string message,
			[CallerMemberName] string method = "",
			[CallerFilePath] string file = "",
			[CallerLineNumber] int lineNumber = 0) => Base(options, LogLevel.Error, message, file, method, lineNumber);

		/// <summary>
		/// Write a critical log.
		/// </summary>
		/// <param name="options">The options for this log entry.</param>
		/// <param name="message">The message/exception you want to log.</param>
		public static LogResult Critical(Options options, string message,
			[CallerMemberName] string method = "",
			[CallerFilePath] string file = "",
			[CallerLineNumber] int lineNumber = 0) => Base(options, LogLevel.Critical, message, file, method, lineNumber);

		/// <summary>
		/// Function to log a message to a .html file.
		/// </summary>
		/// <param name="options">The options for this log entry.</param>
		/// <param name="level">The log level of the message.</param>
		/// <param name="message">The message you want to log.</param>
		public static LogResult Write(Options options, LogLevel level, string message,
			[CallerFilePath] string file = "",
			[CallerMemberName] string method = "",
			[CallerLineNumber] int lineNumber = 0) => Base(options, level, message, file, method, lineNumber);

		/// <summary>
		/// Function to log a message to a .html file.
		/// </summary>
		/// <param name="options">The options for this log entry.</param>
		/// <param name="level">The log level of the message.</param>
		/// <param name="message">The message you want to log.</param>
		/// <param name="file">File where the log occurred.</param>
		/// <param name="method">Method where the log occurred.</param>
		/// <param name="lineNumber">Line number where the log occurred.</param>
		private static LogResult Base(Options options, LogLevel level, string message,
			string file, string method, int lineNumber)
		{
			if (options.Project == null)
				throw new Errors.NoProjectInOptions("You didn't provide any project in your LogToHtml.Options");
			else if (!Configuration.Projects.Contains(options.Project))
				throw new Errors.ProjectsDoesNotContainProject($"None of the projects you provided to LogToHtml.Configuration contains {options.Project}");
			else if (string.IsNullOrEmpty(options.Project))
				throw new Errors.NoProjectInOptions("You haven't set the project that is currently used for logging in LogToHtml.Options");
			else
			{
				if (options.LogToConsole)
				{
					StringBuilder logMessage = new();
					Configuration.ConsoleConfig config = Configuration.ConsoleConfiguration;

					string color = string.Empty;

					if (config.Date)
						logMessage.Append(Markup.Escape($"[{Configuration.Date}] "));
					if (config.LogLevel)
					{
						color = Colors.Get(level);
						logMessage.Append("[[");
						logMessage.Append($"[{color}]{level}[/]");
						logMessage.Append("]] ");
					}
					if (config.ProjectName)
						logMessage.Append(Markup.Escape($"[{options.Project}] "));
					if (config.FileName && !string.IsNullOrEmpty(file))
						logMessage.Append(Markup.Escape($"[{Path.GetFileName(file)}] "));
					if (config.MethodName && !string.IsNullOrEmpty(method))
						logMessage.Append(Markup.Escape($"[{method}] "));
					if (config.LineNumber)
						logMessage.Append(Markup.Escape($"[{lineNumber}] "));

					string header = logMessage.ToString().Trim();
					if (config.LogLevel)
					{
						// Write with color
						try
						{
							// Hex color
							AnsiConsole.MarkupLine($"{header} {Markup.Escape(message)}");
						}
						catch
						{
							try
							{
								// RGB color
								AnsiConsole.MarkupLine($"{header.Replace(color, $"rgb({color})")} {Markup.Escape(message)}");
							}
							catch (Exception e)
							{
								Exception? innerException = e.InnerException;
								string exceptionMessage = "The color you provided isn't supported!";

								if (innerException == null)
									throw new Errors.InvalidColorException(exceptionMessage);
								else
									throw new Errors.InvalidColorException(exceptionMessage, innerException);
							}
						}
					}
					else
					{
						// Write without color
						Console.WriteLine($"{header} {message}");
					}
				}

				bool fileExists = File.Exists(Configuration.LogFile.Full);
				switch (fileExists)
				{
					case false:
						// Create New file
						Logs.Create(options, level, message);
						break;
					default:
						// Edit existing file
						Logs.Edit(options, level, message);
						break;
				}
			}
			return new() { Date = Configuration.Date, LogLevel = level, Message = message };
		}

		/// <summary>
		/// Returns all of the errors logged so far.
		/// </summary>
		public static GetLogs GetLogs()
		{
			return new()
			{
				Info = AllLogs.Info,
				Warn = AllLogs.Warn,
				Error = AllLogs.Error,
				Critical = AllLogs.Critical,
			};
		}

		/// <summary>
		/// Returns all log entries with the info LogLevel.
		/// </summary>
		public static List<LogData> GetInfoLogs() => AllLogs.Info;

		/// <summary>
		/// Returns all log entries with the warn LogLevel.
		/// </summary>
		public static List<LogData> GetWarnLogs() => AllLogs.Warn;

		/// <summary>
		/// Returns all log entries with the error LogLevel.
		/// </summary>
		public static List<LogData> GetErrorLogs() => AllLogs.Error;

		/// <summary>
		/// Returns all log entries with the critical LogLevel.
		/// </summary>
		public static List<LogData> GetCriticalLogs() => AllLogs.Critical;
	}
}
