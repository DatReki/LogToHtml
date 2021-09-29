using RazorLight;
using System.Threading.Tasks;
using static LogToHtml.Logging;
using System.Collections.Generic;

namespace LogToHtml.Core.Create
{
	internal class Render
	{
        private static readonly RazorLightEngine _engine = new RazorLightEngineBuilder()
            .UseEmbeddedResourcesProject(typeof(Render))
            .SetOperatingAssembly(typeof(Render).Assembly)
            .UseMemoryCachingProvider()
            .Build();

        /// <summary>Use RazorLight to get the embedded .cshtml file and convert it to a string</summary>
        internal static async Task<string> RenderViewAsync(Options options, LogType logType, string error)
		{
            LogModel model = new()
            {
                Projects = options.Projects,
                Project = options.Project,
                LogTypes = new List<string>()
                {
                    LogType.Critical.ToString(),
                    LogType.Error.ToString(),
                    LogType.Warn.ToString(),
                    LogType.Info.ToString()
                },
                LogType = logType,
                Date = options.Date,
                Error = error
            };

            return await _engine.CompileRenderAsync("Views.MainPage", model);
        }
	}
}
