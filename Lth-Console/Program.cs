using LogToHtml;
using LogToHtml.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Log2HtmlTester
{
	class Program
	{
		#region Set options for LogToHtml
		public static Logging.Options Options { get => options; private set => options = value; }

		private static Logging.Options options = new()
		{
			Project = $"{Assembly.GetCallingAssembly().GetName().Name}",
			LogToConsole = true
		};
		#endregion

		public class Testing
		{
			public Logging.LogLevel LogLevel { get; set; }
			public string Error { get; set; }
		}

		static void Main(string[] args)
		{
			#region Default LogToHtml config
			List<string> projects = new()
			{
				$"{Assembly.GetCallingAssembly().GetName().Name}"
			};

			_ = new Configuration(projects);
			Run();
			#endregion

			#region Change ConsoleConfig
			Configuration.ConsoleConfig consoleConfig = new()
			{
				Date = true,
				FileName = true,
				LineNumber = false,
				LogLevel = true,
				MethodName = false,
				ProjectName = true,
			};

			_ = new Configuration(projects, consoleConfig: consoleConfig);
			Run();
			#endregion

			#region Change colors
			Configuration.Colors colors = new()
			{
				Info = "0, 255, 255",
				Warn = "0,95,95",
				Error = "#5f0000",
				Critical = "#d75f00"
			};

			_ = new Configuration(projects, colors: colors);
			Run();
			#endregion
		}

		private static void Run()
		{
			Stopwatch s = new();
			s.Start();
			Console.WriteLine($"{RunRandomByAmount(100)} runs completed which took {s.Elapsed} time");
			s.Restart();

			Logs logs = Logging.GetLogs();
			Console.WriteLine($"Total amount of logs gotten: {logs.Info.Count + logs.Warn.Count + logs.Error.Count + logs.Critical.Count}");
			Console.WriteLine($"Getting logs took: {s.Elapsed}\n\n\n");
			s.Reset();
		}

		private static int RunRandomByAmount(int logAmount)
		{
			List<Testing> testing = new() { };
			Random random = new();

			Array values = Enum.GetValues(typeof(Logging.LogLevel));
			const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

			int runs = 0;
			for (int i = 0; i < logAmount; i++)
			{
				Logging.LogLevel randomLogType = (Logging.LogLevel)values.GetValue(random.Next(values.Length));
				string error = new(Enumerable.Repeat(chars, 20).Select(s => s[random.Next(s.Length)]).ToArray());
				testing.Add(new Testing { LogLevel = randomLogType, Error = error });
				runs++;
			}

			testing.ForEach(x => Logging.Log(options, x.LogLevel, x.Error));
			return runs;
		}
	}
}
