using AngleSharp.Html;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace LogToHtml.Core
{
	internal static class Extensions
	{
		private static readonly ReaderWriterLockSlim _readWriteLock = new();
		private static StreamWriter _sw = StreamWriter.Null;

		/// <summary>
		/// Check if a directory exists or not
		/// </summary>
		/// <param name="path">Path to a file/directory</param>
		/// <param name="directory">If true returns the directories name</param>
		/// <returns></returns>
		internal static bool TryGetDirectoryName(this string path, out string directory)
		{
			string? x = Path.GetDirectoryName(path);
			if (x == null)
			{
				directory = string.Empty;
				return false;
			}
			else
			{
				directory = x;
				return true;
			}
		}

		/// <summary>
		/// Add all numbers in a string that appear in a row to a integer list.
		/// If other characters appear between numbers add the next set to a new index.
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		internal static List<int> SplitNumbersInString(this string s)
		{
			s = s.Trim();
			List<int> result = new();
			try
			{
				List<int> numbers = new();

				for (int count = 0; count < s.Length; count++)
				{
					char c = s[count];
					if ((count + 1) == s.Length)
					{
						if (char.IsNumber(c))
							numbers.Add(int.Parse(c.ToString()));

						result.Add(int.Parse(numbers.IntToString()));
					}
					else if (char.IsNumber(c))
					{
						numbers.Add(int.Parse(c.ToString()));
					}
					else if (numbers.Count > 0)
					{
						result.Add(int.Parse(numbers.IntToString()));
						numbers.Clear();
					}
				}
			}
			catch { }
			return result;
		}

		/// <summary>
		/// Convert a list of integers to a string
		/// </summary>
		/// <param name="numbers"></param>
		/// <returns></returns>
		internal static string IntToString(this List<int> numbers)
		{
			StringBuilder y = new();
			numbers.ForEach(x => y.Append(x));
			return y.ToString();
		}

		/// <summary>
		/// Write HTML string to log file.
		/// </summary>
		/// <param name="content">HTML.</param>
		internal static void WriteToFile(this string content)
		{
			// Set Status to Locked
			_readWriteLock.EnterWriteLock();
			try
			{
				// Append text to the file
				using (_sw = File.CreateText(Configuration.LogFile.Full))
				{
					_sw.WriteLine(content);
				}
			}
			finally
			{
				// Release lock
				_readWriteLock.ExitWriteLock();
			}
		}

		/// <summary>
		/// Use AngleSharp & Regex to format our HTML.
		/// </summary>
		internal static string FormatHtml(this string html)
		{
			string result = string.Empty;
			HtmlParser parser = new();
			IHtmlDocument documentParser = parser.ParseDocument(html);

			using (StringWriter writer = new())
			{
				documentParser.ToHtml(writer, new PrettyMarkupFormatter
				{
					Indentation = "\t",
					NewLine = "\n"
				});

				//Remove empty new lines from the HTML.
				//Not sure what causes it but by default file with have a lot of random newlines
				result = Regex.Replace(writer.ToString(), @"^\s+$[\r\n]*", string.Empty, RegexOptions.Multiline);
			}
			return result;
		}

		/// <summary>
		/// Convert TimeZoneInfo to DateTime
		/// </summary>
		internal static DateTime GetTime(this TimeZoneInfo timezone) => TimeZoneInfo.ConvertTime(DateTime.Now, timezone);
	}
}
