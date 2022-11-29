using LogToHtml.Models;
using System.Collections.Generic;

namespace LogToHtml.Core
{
	internal class Data
	{
		/// <summary>
		/// Lists of all different logs.
		/// </summary>
		internal class AllLogs
		{
			/// <summary>
			/// All logs with Info LogType.
			/// </summary>
			internal static readonly List<LogData> Info = new();

			/// <summary>
			/// All logs with Warn LogType.
			/// </summary>
			internal static readonly List<LogData> Warn = new();

			/// <summary>
			/// All logs with Error LogType.
			/// </summary>
			internal static readonly List<LogData> Error = new();

			/// <summary>
			/// All logs with Critical LogType.
			/// </summary>
			internal static readonly List<LogData> Critical = new();
		}
	}
}
