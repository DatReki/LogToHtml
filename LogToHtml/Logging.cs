using System;
using System.IO;
using System.Drawing;
using System.Collections.Generic;

using Pastel;
using LogToHtml.Core;
using LogToHtml.Models;

namespace LogToHtml
{
    public class Logging
    {
        public class Options
        {
            public string FilePath = Path.Combine(Environment.CurrentDirectory, "logging", "loggin.html");
            public List<string> Projects { get; set; }
            public string Project { get; set; }
            public bool LogToConsole = false;
            public DateTime Date = DateTime.UtcNow;
        }

        public enum LogType
        {
            Info,
            Warn,
            Error,
            Critical,
        }

        internal static string GlobalFilePath { get; set; }

        /// <summary>Function to log an error to a .html file</summary>
        /// <param name="options">The (global) options you specified with Logging.Options options = new() { };</param>
        /// <param name="logType">The log level of the error</param>
        /// <param name="message">A string of the error/exception you want to log</param>
        /// <returns>
        ///		<description>(DateTime date, LogType logType, string error)</description>
        ///     <list type="number">
        ///         <item>
        ///             <description>"date" DateTime. Date of the error</description>
        ///         </item>
        ///         <item>
        ///             <description>"logType" LogType. Log level of the error</description>
        ///         </item>
        ///         <item>
        ///             <description>"error" string. A string containing the error/exception</description>
        ///         </item>
        ///     </list>
        /// </returns>
        public static LogResult Log(Options options, LogType logType, string message)
        {
            if (options.Projects == null || options.Projects.Count == 0)
                throw new Errors.Logging.NoProjectsInOptions("You haven't set any projets in your LogToHtml.Options");
            else if (string.IsNullOrEmpty(options.Project))
                throw new Errors.Logging.NoProjectInOptions("You haven't set the project that is currently used for logging in LogToHtml.Options");
            else if (string.IsNullOrEmpty(options.FilePath))
                throw new ArgumentNullException("The file path set in your LogToHtml.Options is null");
            else
            {
                if (options.Projects.Contains(options.Project))
                {
                    GlobalFilePath = options.FilePath;
                    //If options.LogToConsole == true log the error to console also (with colors)
                    if (options.LogToConsole)
                    {
                        string consoleError = null;
                        switch (logType)
                        {
                            case LogType.Critical:
                                consoleError = "Critical".Pastel(Color.Red);
                                break;
                            case LogType.Error:
                                consoleError = "Error".Pastel(Color.Orange);
                                break;
                            case LogType.Warn:
                                consoleError = "Warning".Pastel(Color.Yellow);
                                break;
                            case LogType.Info:
                                consoleError = "Info".Pastel(Color.Green);
                                break;
                        }
                        Console.WriteLine($"[{options.Project}] [{options.Date}] [{consoleError}] {message}");
                    }

                    bool fileExists = File.Exists(options.FilePath);
                    switch (fileExists)
                    {
                        case false:
                            // Create New file
                            WriteLog.CreateLog(options, logType, message);
                            break;
                        default:
                            WriteLog.EditLog(options, logType, message);
                            // Edit existing file
                            break;
                    }
                }
                else
                {
                    throw new Errors.Logging.ProjectsDoesNotContainProject("The project you specified in LogToHtml.Options does not exist in the projects list");
                }
            }
            return new() { Date = options.Date, LogType = logType, Message = message };
        }

        /// <summary>Returns all of the errors logged so far</summary>
        /// <returns>
        ///		<description>(List<AllLogs.LogVariables> Critical, List<AllLogs.LogVariables> Error, List<AllLogs.LogVariables> Warning, List<AllLogs.LogVariables> Info)</description>
        ///     <list type="number">
        ///         <item>
        ///             <description>"Critical" LogToHtml.Core.AllLogs.LogVariables. All of the errors with a critial log level</description>
        ///         </item>
        ///         <item>
        ///             <description>"Error" LogToHtml.Core.AllLogs.LogVariables. All of the errors with a error log level</description>
        ///         </item>
        ///         <item>
        ///             <description>"Warning" LogToHtml.Core.AllLogs.LogVariables. All of the errors with a warning log level</description>
        ///         </item>
        ///         <item>
        ///             <description>"Info" LogToHtml.Core.AllLogs.LogVariables. All of the errors with a info log level</description>
        ///         </item>
        ///     </list>
        /// </returns>
        public static (List<LogData> Critical, List<LogData> Error, List<LogData> Warning, List<LogData> Info) GetLogs()
        {
            return (AllLogs.Critical, AllLogs.Error, AllLogs.Warn, AllLogs.Info);
        }
    }
}
