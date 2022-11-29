using LogToHtml;
using Microsoft.AspNetCore.Mvc;

namespace Lth_Web.Controllers
{
	public class LogsController : Controller
	{
		public IActionResult Index()
		{
			ViewData["wide"] = true;
			ViewData["html"] = System.IO.File.ReadAllText(Startup.LogFilePath);
			return View();
		}

		public IActionResult Info()
		{
			ViewData["logs"] = Log.GetInfoLogs();
			return View();
		}

		public IActionResult Warn()
		{
			ViewData["logs"] = Log.GetWarnLogs();
			return View();
		}

		public IActionResult Error()
		{
			ViewData["logs"] = Log.GetErrorLogs();
			return View();
		}

		public IActionResult Critical()
		{
			ViewData["logs"] = Log.GetCriticalLogs();
			return View();
		}
	}
}
