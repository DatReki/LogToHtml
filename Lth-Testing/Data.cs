using LogToHtml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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

		internal static string Full() => Path.Join(LogPath, LogFile);
	}
}
