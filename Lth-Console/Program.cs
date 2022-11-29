using Bogus;
using ByteSizeLib;
using LogToHtml;
using LogToHtml.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

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
		public static TimeSpan Duration { get; set; } = new();
		public static int ExpectedTotalLogsWritten { get; set; }
		public static int TotalLogsWritten { get; set; }

		static void Main(string[] args)
		{
			string location = Path.Combine(Environment.CurrentDirectory, "logs", "log.html");
			Console.WriteLine($"Log file location: {location}\n");

			Console.WriteLine($"Please select one of the following options:\ngeneral (1)\nlog integers (2)\nimage example (3)");
			string result = Console.ReadLine();

			List<string> projects = new()
			{
				$"{Assembly.GetCallingAssembly().GetName().Name}"
			};
			int maxSize = (int)ByteSize.FromKiloBytes(75).Bytes;

			if (result == "2")
			{
				_ = new Configuration(projects, location, maxSize: maxSize);

				RunCounter(1200);
			}
			else if (result == "3")
			{
				_ = new Configuration(projects, location);

				ExampleImage();
			}
			else
			{
				#region Default LogToHtml config
				_ = new Configuration(projects, location);
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

				_ = new Configuration(projects, location, consoleConfig: consoleConfig);
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

				_ = new Configuration(projects, location, colors: colors);
				Run();
				#endregion

				#region Set max size

				_ = new Configuration(projects, location, maxSize);
				Run();
				#endregion
			}

			Console.WriteLine($"Time it took for all runs: {Duration.Seconds} seconds\n" +
				$"How many logs are supposed to be written: {ExpectedTotalLogsWritten}\n" +
				$"How many logs are actually written: {TotalLogsWritten}\n");
		}

		/// <summary>
		/// Write 100 logs and check how long it took.
		/// </summary>
		private static void Run(int? runs = null)
		{
			Stopwatch s = new();
			int amount;
			int random = Random.Next(1, 2);
			int result;

			if (runs == null)
				amount = 100;
			else
				amount = (int)runs;

			ExpectedTotalLogsWritten += amount;
			s.Start();

			if (random == 1)
				result = WriteRandomLogs(amount);
			else
				result = WriteFakeUserLogs(amount);

			TimeSpan elapsed = s.Elapsed;
			Duration += elapsed;
			TotalLogsWritten += result;
			Console.WriteLine($"{result} runs completed which took {elapsed.TotalSeconds} seconds");
			s.Restart();

			GetLogs logs = Log.GetLogs();
			int logCount = logs.Info.Count + logs.Warn.Count + logs.Error.Count + logs.Critical.Count;
			Console.WriteLine($"Total amount of logs gotten: {logCount}");
			Console.WriteLine($"Getting logs took: {s.Elapsed.Seconds} seconds\n\n");
			s.Reset();
		}

		public static void RunCounter(int? runs)
		{
			Stopwatch s = new();
			int amount;
			int result = 0;

			if (runs == null)
				amount = 100;
			else
				amount = (int)runs;

			ExpectedTotalLogsWritten += amount;
			s.Start();

			for (int i = 0; i < amount; i++)
			{
				Log.Info(options, $"{i}");
				result++;
			}

			TimeSpan elapsed = s.Elapsed;
			Duration += elapsed;
			TotalLogsWritten += result;
			Console.WriteLine($"{result} runs completed which took {elapsed.TotalSeconds} seconds");
			s.Restart();

			GetLogs logs = Log.GetLogs();
			int logCount = logs.Info.Count + logs.Warn.Count + logs.Error.Count + logs.Critical.Count;
			Console.WriteLine($"Total amount of logs gotten: {logCount}");
			Console.WriteLine($"Getting logs took: {s.Elapsed.Seconds} seconds\n\n");
			s.Reset();
		}

		public static void ExampleImage()
		{
			Log.Info(options, "Information");
			Log.Warn(options, "Warning");
			Log.Error(options, "Error");
			Log.Critical(options, "Critical");
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
	}
}
