using LogToHtml;

namespace Lth_Testing
{
	public class Data
	{
		public static string LogPath { get; set; } = string.Empty;
		public static string LogFile { get; set; } = string.Empty;

		public static Log.Options Options { get => options; private set => options = value; }
		private static Log.Options options = new()
		{
			Project = $"UnitTesting",
			LogToConsole = false
		};

		public static List<string> Projects { get => projects; private set => projects = value; }
		private static List<string> projects = new()
		{
			options.Project
		};

		internal static string Full() => Path.Join(LogPath, LogFile);
	}
}
