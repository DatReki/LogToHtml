using LogToHtml;
using System.Reflection;

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

			List<string> projects = new()
			{
				Data.Options.Project
			};

			_ = new Configuration(projects, Data.Full());

			Directory.CreateDirectory(Data.LogPath);
		}

		[OneTimeTearDown]
		public void TearDown()
		{
			Directory.Delete(Data.LogPath, true);
		}
	}
}