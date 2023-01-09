# LoToHtml

<a href="https://github.com/DatReki/LogToHtml/actions/workflows/dotnet.yml">
    <img src="https://github.com/DatReki/LogToHtml/actions/workflows/dotnet.yml/badge.svg" />
</a>
<a href="https://www.nuget.org/packages/LogToHtml/">
    <img src="https://img.shields.io/nuget/v/LogToHtml?style=flat-square" />
</a>
<a href="https://www.paypal.com/donate?hosted_button_id=WRETYRRSJ4T2L">
    <img src="https://img.shields.io/badge/Donate-PayPal-green.svg?style=flat-square">
</a>

A small library to write logs to a .html file.
The HTML file's structure is based on an embedded .cshtml file.

<img src="https://i.imgur.com/pzM1BDj.png" alt="LogToHtml.png">

## Usage

Write a log

```cs
using LogToHtml;
using System.Reflection;
namespace Example
{
    internal class Program
    {
        #region Configure options
        // For each individual log entry
        public static Log.Options Options = new()
        {
            // Name of the project that you're currently logging from.
            // If you have a solution with multiple projects,
            // you would change this value for each different project.
            Project = $ "{Assembly.GetCallingAssembly().GetName().Name}",
			
			// Indicate if you just want to write to the HTML file or also output results on the console.
			LogToConsole = true
        };
        #endregion

        static void Main(string[] args)
        {
            #region Configure global options
            // These are applied across all projects in a solution.

            // [Required]
            // A List of projects that the logger is used for.
            // If you only use the logger in a single project assign the same value here as you did in 'Options.Project'
            List < string > projects = new()
            {
                $ "{Assembly.GetCallingAssembly().GetName().Name}"
            };

            // [Optional]
            // The path where the log file is located.
            string logpath = Path.Combine(Environment.CurrentDirectory, "logs", "log.html");

            // [Optional]
            // Set the maximum size of a log file (example has a max size of 1 MB).
            int maxSize = 1000000;

            // [Optional]
            // Set what timezone the library uses.
            TimeZoneInfo timezone = TimeZoneInfo.FindSystemTimeZoneById("Central America Standard Time");

            // [Optional]
            // Change what colors the library writes to the console based on LogLevel.
            Configuration.Colors colors = new()
            {
                Info = "0, 255, 255",
				Warn = "0,95,95",
				Error = "#5f0000",
				Critical = "#d75f00"
            };

            // [Optional]
            // Change what get's written to the console.
            Configuration.ConsoleConfig consoleConfig = new()
            {
                // If console displays LogLevel in color
                Color = true,
				Date = true,
				FileName = true,
				LineNumber = false,
				LogLevel = true,
				MethodName = false,
				ProjectName = true
            };

            // You only need to set this once in your entire solution.
            _ = new Configuration(projects, logpath, maxSize, timezone, colors, consoleConfig);
            #endregion

            // And now you can start logging
            Log.Info(Options, $ "This is a test message");
        }
    }
}
```

Retrieve written logs

```cs
using LogToHtml.Models;

// Get all logs
GetLogs allLogs = Log.GetLogs();
// Get all logs with the info LogLevel
List<LogData> infoLogs = Log.GetInfoLogs();
// Get all logs with the warn LogLevel
List<LogData> warnLogs = Log.GetWarnLogs();
// Get all logs with the error LogLevel
List<LogData> errorLogs = Log.GetErrorLogs();
// Get all logs with the critical LogLevel
```

## Performance

Currently if you're just writing a log every so often performance is fine but if you write a massive amount to it (300-1000+) it will slow down significantly. I do know ways to make the library faster but currently do not have time to implement these.

## To Do

- [ ] Make sure the library has read access to the file it's logging to if not wait for it to become accessible.
- [ ] Make the logging process happen in threads this way it won't cause delays for the program that's writing the logs.
- [x] Store the edited HTML as a string inside the library so that we only need to read logging file once (if it exists).
- [ ] Create a queue system for the logs to reduce the amount of IO calls the library needs to make.
