using LogToHtml.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace LogToHtml
{
	public class Configuration
	{
		/// <summary>
		/// List of projects that can be logged to.
		/// </summary>
		internal static List<string> Projects { get; set; } = new();

		/// <summary>
		/// Information about the file being logged to.
		/// </summary>
		internal static LogFileInfo LogFile { get; set; } = new LogFileInfo();

		/// <summary>
		/// The maximum size of the file being logged to.
		/// </summary>
		internal static int MaxSize { get; set; } = 0;

		/// <summary>
		/// The DateTime used for log entries.
		/// </summary>
		internal static TimeZoneInfo TimeZone { get; private set; } = TimeZoneInfo.Local;

		/// <summary>
		/// The colors used for log entries.
		/// </summary>
		internal static Colors ConsoleColors { get; private set; } = new Colors();

		/// <summary>
		/// Configure what get's shown when the library logs to the console.
		/// </summary>
		internal static ConsoleConfig ConsoleConfiguration { get; set; } = new ConsoleConfig();

		/// <summary>
		/// Maximum size of a log file (1 GigaByte).
		/// </summary>
		private static readonly int _absoluteMaxSize = 1000000000;

		/// <summary>
		/// Assign colors to different log levels.
		/// </summary>
		public class Colors
		{
			public string Info { get; set; } = "#00ff00";
			public string Warn { get; set; } = "#ffff00";
			public string Error { get; set; } = "#ff8700";
			public string Critical { get; set; } = "#ff0000";
		}

		/// <summary>
		/// Configure what get's shown when the library logs to the console.
		/// </summary>
		public class ConsoleConfig
		{
			/// <summary>
			/// Configure if you want LogLevel should be displayed in color or not.
			/// </summary>
			public bool Color { get; set; } = true;

			/// <summary>
			/// Configure if the console should display the date.
			/// </summary>
			public bool Date { get; set; } = true;

			/// <summary>
			/// Configure if the console should display the LogLevel.
			/// </summary>
			public bool LogLevel { get; set; } = true;

			/// <summary>
			/// Configure if the console should display the name of the project currently being logged to.
			/// </summary>
			public bool ProjectName { get; set; } = true;

			/// <summary>
			/// Configure if the console should display the filename of the file where the log occurred.
			/// </summary>
			public bool FileName { get; set; } = true;

			/// <summary>
			/// Configure if the console should display the name of the method where the log occurred.
			/// </summary>
			public bool MethodName { get; set; } = true;

			/// <summary>
			/// Configure if the console should display the line number where the log occurred.
			/// </summary>
			public bool LineNumber { get; set; } = true;
		}

		internal class Check
		{
			internal enum Types
			{
				None,
				Hex,
				Rgb,
			}

			internal bool Result { get; set; }
			internal Types Type { get; set; }
			internal string Color { get; set; } = string.Empty;
		}

		/// <summary>
		/// Information about the log file.
		/// </summary>
		internal class LogFileInfo
		{
			/// <summary>
			/// Full path to the log file.
			/// </summary>
			internal string Full { get; set; } = string.Empty;

			/// <summary>
			/// Directory of the log file.
			/// </summary>
			internal string Directory { get; set; } = string.Empty;

			/// <summary>
			/// The log file's filename.
			/// </summary>
			internal string FileName { get; set; } = string.Empty;

			/// <summary>
			/// The log file's extension.
			/// </summary>
			internal string Extension { get; set; } = string.Empty;
		}

		/// <summary>
		/// Configure your settings for LogToHtml.
		/// </summary>
		/// <param name="projects">All of the different projects you want to use the logger for.</param>
		/// <param name="logPath">Path of the file that you want to log to.</param>
		/// <param name="maxSize">Maximum size of the log file (in bytes). This cannot be more than 1GB.</param>
		/// <param name="timeZone">What timezone you want the application to use.</param>
		/// <param name="colors">What colors you want to show for each different log level.</param>
		/// <param name="consoleConfig">Configure what get's shown when the library logs to the console.</param>
		/// <exception cref="Errors.NoProjectsInConfig">Thrown when you don't provide any projects.</exception>
		/// <exception cref="Errors.InvalidColorException">Thrown when you pass invalid (HEX/RGB) colors.</exception>
		public Configuration(List<string> projects, string? logPath = null, int? maxSize = null, TimeZoneInfo? timeZone = null, Colors? colors = null,
			ConsoleConfig? consoleConfig = null)
		{
			if (projects.Count == 0)
				throw new Errors.NoProjectsInConfig("You didn't provide any projects in your configuration");
			else
				Projects = projects;

			// If log path === null than
			logPath ??= Path.Combine(Environment.CurrentDirectory, "logging", "loggin.html");
			logPath.TryGetDirectoryName(out string directory);
			Directory.CreateDirectory(directory);

			LogFile = new LogFileInfo()
			{
				Full = logPath,
				Directory = directory,
				FileName = Path.GetFileNameWithoutExtension(logPath),
				Extension = Path.GetExtension(logPath)
			};

			if (maxSize == null)
				maxSize = _absoluteMaxSize;
			else
			{
				if (maxSize > _absoluteMaxSize)
					maxSize = _absoluteMaxSize;
			}
			MaxSize = (int)maxSize;

			if (timeZone != null)
				TimeZone = timeZone;
			if (colors != null)
			{
				foreach (PropertyInfo? property in colors.GetType().GetProperties())
				{
					if (property != null)
					{
						object? value = property.GetValue(colors, null);
						if (value != null)
						{
							string? color = value.ToString();
							if (color != null)
							{
								Check colorCheck = CheckIfColor(color);
								if (colorCheck.Type == Check.Types.Rgb)
									property.SetValue(colors, colorCheck.Color);
								else if (!colorCheck.Result)
									throw new Errors.InvalidColorException($"{color} is not a valid hex or rgb color!");
							}
						}
					}
				}
				ConsoleColors = colors;
			}
			if (consoleConfig != null)
				ConsoleConfiguration = consoleConfig;
		}

		/// <summary>
		/// Check if string contains a valid RGB or HEX value.
		/// </summary>
		/// <param name="text">String containing RGB or HEX value.</param>
		/// <returns>Data about the result.</returns>
		internal static Check CheckIfColor(string text)
		{
			bool hex = IfValidHex(text);
			bool rgb = false;
			int r = 0;
			int g = 0;
			int b = 0;

			List<int> rgbNumbers = text.SplitNumbersInString();
			if (!hex && rgbNumbers.Count == 3)
			{
				r = rgbNumbers[0];
				g = rgbNumbers[1];
				b = rgbNumbers[2];
				rgb = IfValidRgb(r, g, b);
			}

			if (hex)
				return new Check() { Result = true, Type = Check.Types.Hex };
			else if (rgb)
				return new Check() { Result = true, Type = Check.Types.Rgb, Color = $"rgb({r},{g},{b})" };
			else
				return new Check() { Result = false };
		}

		/// <summary>
		/// Check if hex value is valid.
		/// </summary>
		/// <param name="text">String containing a HEX value.</param>
		/// <returns></returns>
		internal static bool IfValidHex(string text) => Regex.IsMatch(text, @"[#][0-9A-Fa-f]{6}\b");

		/// <summary>
		/// Check if provided values together represent a valid RGB value.
		/// </summary>
		/// <param name="r">Red value.</param>
		/// <param name="g">Green value.</param>
		/// <param name="b">Blue value.</param>
		/// <returns></returns>
		public static bool IfValidRgb(int r, int g, int b)
		{
			if (r < 0 || r > 255)
				return false;
			else if (g < 0 || g > 255)
				return false;
			else if (b < 0 || b > 255)
				return false;
			else
				return true;
		}
	}
}
