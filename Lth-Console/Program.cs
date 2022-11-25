using Bogus;
using LogToHtml;
using LogToHtml.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using static Log2HtmlTester.Program;

namespace Log2HtmlTester
{
	class Program
	{
		#region Set options for LogToHtml
		public static Log.Options Options { get => options; private set => options = value; }

		private static Log.Options options = new()
		{
			Project = $"{Assembly.GetCallingAssembly().GetName().Name}",
			LogToConsole = true
		};
		#endregion

		public class Testing
		{
			public Log.LogLevel LogLevel { get; set; }
			public string Message { get; set; }
		}

		public static Random Random = new();

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

			CheckLogs();
		}

		/// <summary>
		/// Write 100 logs and check how long it took.
		/// </summary>
		private static void Run()
		{
			Stopwatch s = new();
			int result;
			int amount = 100;
			int random = Random.Next(0, 1);

			s.Start();
			if (random == 0)
				result = WriteRandomLogs(amount);
			else
				result = WriteFakeUserLogs(amount);

			Console.WriteLine($"{result} runs completed which took {s.Elapsed.TotalSeconds} seconds");
			s.Restart();

			GetLogs logs = Log.GetLogs();
			Console.WriteLine($"Total amount of logs gotten: {logs.Info.Count + logs.Warn.Count + logs.Error.Count + logs.Critical.Count}");
			Console.WriteLine($"Getting logs took: {s.Elapsed.Seconds} seconds\n\n");
			s.Reset();
		}

		/// <summary>
		/// Write random garbage to logs.
		/// </summary>
		/// <param name="amount">The amount of garbage logs you want to generate.</param>
		public static int WriteRandomLogs(int amount)
		{
			string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
			List<string> messages = new();

			for (int i = 0; i < amount; i++)
				messages.Add(new string(Enumerable.Repeat(chars, 20).Select(s => s[Random.Next(s.Length)]).ToArray()));

			return WriteLogs(messages);
		}

		/// <summary>
		/// Write fake user creation logs.
		/// </summary>
		/// <param name="amount">Amount of fake user creation logs you want to generate.</param>
		public static int WriteFakeUserLogs(int amount)
		{
			Faker data = new();
			List<string> messages = new();

			for (int i = 0; i < amount; i++)
				messages.Add($"Created new user!\n" +
					$"Username: {data.Person.UserName}\n" +
					$"Firstname: {data.Person.FirstName}\n" +
					$"Lastname: {data.Person.LastName}\n" +
					$"Email: {data.Person.Email}\n" +
					$"Address: {data.Person.Address}\n" +
					$"File: {Path.GetRandomFileName()}");

			return WriteLogs(messages);
		}

		/// <summary>
		/// Write logs with a random LogLevel.
		/// </summary>
		/// <param name="messages">Messages you want to log.</param>
		private static int WriteLogs(List<string> messages)
		{
			List<Testing> testing = new() { };

			Array values = Enum.GetValues(typeof(Log.LogLevel));
			int runs = 0;

			for (int i = 0; i < messages.Count; i++)
			{
				object? value = values.GetValue(Random.Next(values.Length));
				if (value != null)
				{
					Log.LogLevel randomLogType = (Log.LogLevel)value;
					testing.Add(new Testing { LogLevel = randomLogType, Message = messages[i] });
					runs++;
				}
			}

			testing.ForEach(x => Log.Write(Options, x.LogLevel, x.Message));
			return runs;
		}

		private static void CheckLogs()
		{
			int info = Log.GetInfoLogs().Count;
			int warn = Log.GetWarnLogs().Count;
			int error = Log.GetErrorLogs().Count;
			int critical = Log.GetCriticalLogs().Count;

			Console.WriteLine($"All logs: {info + warn + error + critical}\n" +
				$"Info: {info}\n" +
				$"Warn: {warn}\n" +
				$"Error: {error}\n" +
				$"Critical: {critical}");
		}
	}
}
