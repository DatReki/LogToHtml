using System.IO;
using System.Threading;
using System.Text.RegularExpressions;

using AngleSharp.Html;
using AngleSharp.Html.Parser;

namespace LogToHtml.Core
{
    internal static class Extensions
    {
        private static readonly ReaderWriterLockSlim _readWriteLock = new();
        private static StreamWriter _sw;

        /// <summary>Write contents to file</summary>
        internal static void WriteToFile(this string content, string path)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            // Set Status to Locked
            _readWriteLock.EnterWriteLock();
            try
            {
                // Append text to the file
                using (_sw = File.CreateText(path))
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

        /// <summary>Use AngleSharp & Regex to format our HTML</summary>
        internal static string FormatHtml(this string html)
        {
            string result = null;

            var parser = new HtmlParser();
            var documentParser = parser.ParseDocument(html);
            using (var writer = new StringWriter())
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
	}
}
