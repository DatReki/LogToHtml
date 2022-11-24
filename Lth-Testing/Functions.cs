using LogToHtml;

namespace Lth_Testing
{
	internal class Functions
	{
		internal class TestLog
		{
			internal Logging.LogLevel LogLevel { get; set; }
			internal string Message { get; set; } = string.Empty;
		}

		/// <summary>
		/// Write random garbage to logs.
		/// </summary>
		/// <param name="logAmount">The amount of garbage logs you want to generate.</param>
		internal static int RunRandomByAmount(int logAmount)
		{
			List<TestLog> testing = new() { };
			Random random = new();

			Array values = Enum.GetValues(typeof(Logging.LogLevel));
			const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

			int runs = 0;

			for (int i = 0; i < logAmount; i++)
			{
				object? value = values.GetValue(random.Next(values.Length));
				if (value != null)
				{
					Logging.LogLevel randomLogType = (Logging.LogLevel)value;
					string error = new(Enumerable.Repeat(chars, 20).Select(s => s[random.Next(s.Length)]).ToArray());
					testing.Add(new TestLog { LogLevel = randomLogType, Message = error });
					runs++;
				}
			}

			testing.ForEach(x => Logging.Log(Data.Options, x.LogLevel, x.Message));
			return runs;
		}
	}
}
