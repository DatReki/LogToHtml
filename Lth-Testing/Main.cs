using LogToHtml;

namespace Lth_Testing
{
	[SetUpFixture]
	public class Main
	{
		[OneTimeSetUp]
		public void Setup()
		{
			Data.LogFile = "log.html";
			Data.LogPath = Path.Combine(Path.GetTempPath(), "LogToHtml");

			_ = new Configuration(Data.Projects, Data.Full());

			Directory.CreateDirectory(Data.LogPath);
		}

		[OneTimeTearDown]
		public void TearDown()
		{
			Directory.Delete(Data.LogPath, true);
		}
	}
}