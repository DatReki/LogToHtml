using LogToHtml;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Log2HtmlTester
{
	class Program
	{
		public static Logging.Options options = new()
		{
			Projects = new List<string>()
			{
				$"{Assembly.GetCallingAssembly().GetName().Name}"
			},
			Project = $"{Assembly.GetCallingAssembly().GetName().Name}",
			LogToConsole = true
		};

		public class Testing
		{
			public Logging.LogType TLogType { get; set; }
			public string Error { get; set; }
		}

		static void Main(string[] args)
		{
			List<Testing> testing = new() { };
			Random random = new();

			Array values = Enum.GetValues(typeof(Logging.LogType));
			const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

			int runs = 150;

			for (int i = 0; i < runs; i++)
			{
				Logging.LogType randomLogType = (Logging.LogType)values.GetValue(random.Next(values.Length));
				string error = new(Enumerable.Repeat(chars, 20).Select(s => s[random.Next(s.Length)]).ToArray());
				testing.Add(new Testing { TLogType = randomLogType, Error = error });
			}

			Stopwatch s = new();
			s.Start();
			foreach (var item in testing)
			{
				Logging.Log(options, item.TLogType, item.Error);
			}
			Console.WriteLine($"{runs} runs completed which took {s.Elapsed} time");
			s.Restart();
			//Logging.Log(options, Logging.LogType.Warn, "Testing");
			var logs = Logging.GetLogs();
			Console.WriteLine($"Total amount of logs gotten: {logs.Info.Count + logs.Warning.Count + logs.Error.Count + logs.Critical.Count}");
			Console.WriteLine($"Getting logs took: {s.Elapsed}");
			s.Reset();
		}
	}
}
