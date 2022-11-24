using LogToHtml;

namespace Lth_Testing
{
	public class Tests
	{
		/// <summary>
		/// Write our first log.
		/// </summary>
		[Test]
		public void Log()
		{
			Logging.Log(Data.Options, Logging.LogLevel.Info, $"Test");
			Assert.IsTrue(true);
		}

		/// <summary>
		/// Check if log file exists (it should).
		/// </summary>
		[Test]
		public void LogFileDoesExist()
		{
			bool result = File.Exists(Data.Full());
			Assert.That(result, Is.True);
		}

		/// <summary>
		/// Write multiple logs
		/// </summary>
		[Test]
		public void WriteMultipleLogs()
		{
			int toRun = 100;
			int ran = Functions.RunRandomByAmount(toRun);
			Assert.That(ran, Is.EqualTo(toRun));
		}
	}
}
