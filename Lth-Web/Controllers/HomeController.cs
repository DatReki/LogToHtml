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

		public static Random random = new();

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

		public IActionResult AddMultipleLogs()
		{
			if (TempData["postedLogs"] != null)
			{
				ViewData["postedLogs"] = TempData["postedLogs"];
			}
			return View();
		}

		public class GenerateLogsVariables
		{
			public Logging.LogType LogType { get; set; }
			public string Error { get; set; }
		}

		[HttpPost]
		public IActionResult GenerateLogs()
		{
			List<GenerateLogsVariables> logList = new() { };

			Array values = Enum.GetValues(typeof(Logging.LogType));
			const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

			int runs = Int32.Parse(Request.Form["amount"].ToString());

			for (int i = 0; i < runs; i++)
			{
				Logging.LogType randomLogType = (Logging.LogType)values.GetValue(random.Next(values.Length));
				string error = new(Enumerable.Repeat(chars, 20).Select(s => s[random.Next(s.Length)]).ToArray());
				logList.Add(new GenerateLogsVariables { LogType = randomLogType, Error = error });
			}

			List<string> data = new List<string>();

			Stopwatch sw = new Stopwatch();
			sw.Start();
			foreach (GenerateLogsVariables item in logList)
			{
				data.Add(item.Error);
				Logging.Log(options, item.LogType, item.Error);
			}
			sw.Stop();
			data.Add($"Logging took: {sw.Elapsed.TotalSeconds}");

			TempData["postedLogs"] = data;

			return RedirectToAction("AddMultipleLogs", "Home");
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
