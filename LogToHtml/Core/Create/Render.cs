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
        internal static async Task<string> RenderViewAsync()
        {
            LogModel model = new()
            {
                Projects = Configuration.Projects,
                LogTypes = new List<string>()
                {
                    LogLevel.Critical.ToString(),
                    LogLevel.Error.ToString(),
                    LogLevel.Warn.ToString(),
                    LogLevel.Info.ToString()
                },
            };

            return await _engine.CompileRenderAsync("Views.MainPage", model);
        }
    }
}
