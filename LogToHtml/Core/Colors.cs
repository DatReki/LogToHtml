using LogLevel = LogToHtml.Log.LogLevel;

namespace LogToHtml.Core
{
	public class Colors
	{
		/// <summary>
		/// Get the colors used for different log levels
		/// </summary>
		/// <param name="level">The log level</param>
		/// <returns>A string containing a RGB or HEX value</returns>
		internal static string Get(LogLevel level)
		{
			return level switch
			{
				LogLevel.Warn => Configuration.ConsoleColors.Warn,
				LogLevel.Error => Configuration.ConsoleColors.Error,
				LogLevel.Critical => Configuration.ConsoleColors.Critical,
				_ => Configuration.ConsoleColors.Info,
			};
		}
	}
}
