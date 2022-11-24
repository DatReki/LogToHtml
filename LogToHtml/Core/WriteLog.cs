using LogToHtml.Models;
using System;
using System.Collections.Generic;
using static LogToHtml.Logging;

namespace LogToHtml.Core
{
    internal class WriteLog
    {
        internal static bool ReadLogFile = true;
        internal static string Html = string.Empty;

        /// <summary>
        /// Creates the .html file from an embedded .cshtml file. This will be used as our log file.
        /// </summary>
        /// <param name="options">Options for the log entry.</param>
        /// <param name="logType">The logs LogType.</param>
        /// <param name="message">The message associated with the log.</param>
        internal static void CreateLog(Options options, LogLevel logType, string message)
        {
            // Create & format a HTML file from the embedded .cshtml file
            Html = Create.Render.RenderViewAsync().Result.FormatHtml();
            // Add first log entry to the log file
            string logHtml = Edit.EditExistingLog.Edit(options, logType, message).FormatHtml();
            // Indicate we don't need to read from the log file as we just created a new one
            ReadLogFile = false;
            // Add log to list of all logs
            AddToListOfLogs(options.Project, Configuration.Date, logType, message);
            // Update static HTML string
            Html = logHtml;
            // Add new entry to log file
            logHtml.WriteToFile();
        }

        /// <summary>
        /// Edits the .html file containing all of our logs.
        /// </summary>
        /// <param name="options">Options for the log entry.</param>
        /// <param name="logType">The logs LogType.</param>
        /// <param name="message">The message associated with the log.</param>
        internal static void EditLog(Options options, LogLevel logType, string message)
        {
            if (ReadLogFile)
            {
                Read.ReadLogs.ReadLogsFromFile();
                ReadLogFile = false;
            }
            else
                AddToListOfLogs(options.Project, Configuration.Date, logType, message);

            // Add log to HTML string
            string html = Edit.EditExistingLog.Edit(options, logType, message);
            // Format HTML string
            html = html.FormatHtml();
            // Update static HTML string
            Html = html;
            // Write updated HTML string to log file
            html.WriteToFile();
        }

        /// <summary>
        /// Updates the lists containing all log entries.
        /// </summary>
        /// <param name="project">Name of the project the log was written from.</param>
        /// <param name="date">Date the log was written.</param>
        /// <param name="logType">The logs LogType.</param>
        /// <param name="message">The message associated with the log.</param>
        internal static void AddToListOfLogs(string project, DateTime date, LogLevel logType, string message)
        {
            switch (logType)
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
    }

    /// <summary>
    /// Lists of all different logs.
    /// </summary>
    internal class AllLogs
    {
        /// <summary>
        /// All logs with Info LogType.
        /// </summary>
        internal static readonly List<LogData> Info = new();

        /// <summary>
        /// All logs with Warn LogType.
        /// </summary>
        internal static readonly List<LogData> Warn = new();

        /// <summary>
        /// All logs with Error LogType.
        /// </summary>
        internal static readonly List<LogData> Error = new();

        /// <summary>
        /// All logs with Critical LogType.
        /// </summary>
        internal static readonly List<LogData> Critical = new();
    }
}
