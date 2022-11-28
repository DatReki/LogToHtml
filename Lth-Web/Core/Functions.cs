using Bogus;
using LogToHtml;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Lth_Web.Core
{
	public class Functions
	{
		internal class TestLog
		{
			internal Log.LogLevel LogLevel { get; set; }
			internal string Message { get; set; } = string.Empty;
		}

		private static readonly Random _random = new();

		/// <summary>
		/// Write random garbage to logs.
		/// </summary>
		/// <param name="amount">The amount of garbage logs you want to generate.</param>
		public static int WriteRandomLogs(int amount)
		{
			string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
			List<string> messages = new();

			for (int i = 0; i < amount; i++)
				messages.Add(new string(Enumerable.Repeat(chars, 20).Select(s => s[_random.Next(s.Length)]).ToArray()));

			return WriteLogs(messages);
		}

		/// <summary>
		/// Write fake user creation logs.
		/// </summary>
		/// <param name="amount">Amount of fake user creation logs you want to generate.</param>
		/// <returns></returns>
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
					$"Address: {data.Person.Address}\n");

			return WriteLogs(messages);
		}

		/// <summary>
		/// Write logs with a random LogLevel.
		/// </summary>
		/// <param name="messages">Messages you want to log.</param>
		private static int WriteLogs(List<string> messages)
		{
			List<TestLog> testing = new() { };

			Array values = Enum.GetValues(typeof(Log.LogLevel));
			int runs = 0;

			for (int i = 0; i < messages.Count; i++)
			{
				object? value = values.GetValue(_random.Next(values.Length));
				if (value != null)
				{
					Log.LogLevel randomLogType = (Log.LogLevel)value;
					testing.Add(new TestLog { LogLevel = randomLogType, Message = messages[i] });
					runs++;
				}
			}

			testing.ForEach(x => Log.Write(Startup.Options, x.LogLevel, x.Message));
			return runs;
		}
	}
}
