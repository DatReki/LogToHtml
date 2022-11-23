using LogToHtml.Models;
using RazorLight;
using System.Collections.Generic;
using System.Threading.Tasks;
using static LogToHtml.Logging;

namespace LogToHtml.Core.Create
{
    internal class Render
    {
        private static readonly RazorLightEngine _engine = new RazorLightEngineBuilder()
            .UseEmbeddedResourcesProject(typeof(Render))
            .SetOperatingAssembly(typeof(Render).Assembly)
            .UseMemoryCachingProvider()
            .Build();

        /// <summary>
        /// Use RazorLight to get the embedded .cshtml file and convert it to a string.
        /// </summary>
        /// <param name="options">Options for the log entry</param>
        internal static async Task<string> RenderViewAsync(Options options)
        {
            LogModel model = new()
            {
                Projects = options.Projects,
                LogTypes = new List<string>()
                {
                    LogType.Critical.ToString(),
                    LogType.Error.ToString(),
                    LogType.Warn.ToString(),
                    LogType.Info.ToString()
                },
            };

            return await _engine.CompileRenderAsync("Views.MainPage", model);
        }
    }
}
