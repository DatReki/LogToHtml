using LogToHtml;
using LogToHtml.Models;
using System.Text;

namespace Lth_Testing
{
	public class Tests
	{
		/// <summary>
		/// Total amount of times we've written logs.
		/// This is 1 since we write once in WriteLog().
		/// </summary>
		private static int WriteLogCount = 1;

		/// <summary>
		/// Total amount of logs we've retrieved from the library.
		/// </summary>
		private static int GetLogCount = 0;

		/// <summary>
		/// Write our first log.
		/// </summary>
		[Test, Order(1)]
		public void WriteLog()
		{
			Log.Info(Data.Options, $"Test");
			Assert.IsTrue(true);
		}

		/// <summary>
		/// Check if log file exists (it should).
		/// </summary>
		[Test, Order(2)]
		public void LogFileDoesExist()
		{
			bool result = File.Exists(Data.Full());
			Assert.That(result, Is.True);
		}

		/// <summary>
		/// Write multiple logs with random data.
		/// </summary>
		[Test, Order(3)]
		public void WriteRandomLogs()
		{
			int toRun = 200;
			int ran = Functions.WriteRandomLogs(toRun);
			WriteLogCount += toRun;
			Assert.That(ran, Is.EqualTo(toRun));
		}

		/// <summary>
		/// Write multiple logs with fake user data.
		/// </summary>
		[Test, Order(4)]
		public void WriteFakeUserCreationLogs()
		{
			int toRun = 200;
			int ran = Functions.WriteFakeUserLogs(toRun);
			WriteLogCount += toRun;
			Assert.That(ran, Is.EqualTo(toRun));
		}

		/// <summary>
		/// Get info logs.
		/// </summary>
		[Test, Order(5)]
		public void GetInfoLogs()
		{
			List<LogData> logs = Log.GetInfoLogs();
			GetLogCount += logs.Count;
			Assert.That(logs, Is.Not.Empty);
		}

		/// <summary>
		/// Get warn logs.
		/// </summary>
		[Test, Order(6)]
		public void GetWarnLogs()
		{
			List<LogData> logs = Log.GetWarnLogs();
			GetLogCount += logs.Count;
			Assert.That(logs, Is.Not.Empty);
		}

		/// <summary>
		/// Get error logs.
		/// </summary>
		[Test, Order(7)]
		public void GetErrorLogs()
		{
			List<LogData> logs = Log.GetErrorLogs();
			GetLogCount += logs.Count;
			Assert.That(logs, Is.Not.Empty);
		}

		/// <summary>
		/// Get critical logs.
		/// </summary>
		[Test, Order(8)]
		public void GetCriticalLogs()
		{
			List<LogData> logs = Log.GetCriticalLogs();
			GetLogCount += logs.Count;
			Assert.That(logs, Is.Not.Empty);
		}

		/// <summary>
		/// Check if the amount of logs we've written equals to the amount we've retrieved.
		/// </summary>
		[Test, Order(9)]
		public void WrittenLogsEqualsGottenLogs()
		{
			Assert.That(WriteLogCount, Is.EqualTo(GetLogCount));
		}
	}
}
