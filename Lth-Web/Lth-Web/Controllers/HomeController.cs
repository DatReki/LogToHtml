using LogToHtml;
using Lth_Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Lth_Web.Controllers
{
	public class HomeController : Controller
	{
		public static Logging.Options options = new()
		{
			FilePath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "logging", "loggin.html"),
			Projects = new List<string>()
			{
				$"{Assembly.GetCallingAssembly().GetName().Name}"
			},
			Project = $"{Assembly.GetCallingAssembly().GetName().Name}",
			LogToConsole = true
		};

		public IActionResult Index()
		{
			if (TempData["postedAccountForm"] != null)
			{
				ViewData["postedAccountForm"] = TempData["postedAccountForm"];
			}
			return View();
		}

		[HttpPost]
		public IActionResult Login()
		{
			string firstname = Request.Form["firstname"].ToString();
			string lastname = Request.Form["lastname"].ToString();
			string username = Request.Form["username"].ToString();
			string invalid = Request.Form["invalidcheck"].ToString();

			Stopwatch sw = new Stopwatch();
			sw.Start();
			//This will cause a UI lock when the logging file is being created.
			//This is why I'd ideally use threading but currently cannot figure a good way out to handle it.
			Logging.Log(options, Logging.LogType.Info, $"Created new user: {username}");
			sw.Stop();

			List<string> data = new List<string>();
			data.Add(firstname);
			data.Add(lastname);
			data.Add(username);
			data.Add(invalid);
			data.Add($"{sw.Elapsed.TotalSeconds} seconds");
			sw.Reset();

			TempData["postedAccountForm"] = data;

			return RedirectToAction("Index", "Home");
		}

		public IActionResult Reset()
		{
			return RedirectToAction("Index", "Home");
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
