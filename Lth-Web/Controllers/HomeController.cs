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
		private static readonly Random _random = new();

		public IActionResult Index()
		{
			if (TempData["postedAccountForm"] != null)
				ViewData["postedAccountForm"] = TempData["postedAccountForm"];

			return View();
		}

		[HttpPost]
		public IActionResult Login()
		{
			string firstname = Request.Form["firstname"].ToString();
			string lastname = Request.Form["lastname"].ToString();
			string username = Request.Form["username"].ToString();
			string invalid = Request.Form["invalidcheck"].ToString();

			Stopwatch sw = new();
			sw.Start();
			Logging.Log(Startup.Options, Logging.LogLevel.Info, $"Created new user: {username}");
			sw.Stop();

			List<string> data = new()
			{
				firstname,
				lastname,
				username,
				invalid,
				$"{sw.Elapsed.TotalSeconds} seconds"
			};
			sw.Reset();

			TempData["postedAccountForm"] = data;

			return RedirectToAction("Index", "Home");
		}

		public IActionResult Reset() => RedirectToAction("Index", "Home");

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
			public Logging.LogLevel LogType { get; set; }
			public string Error { get; set; }
		}

		[HttpPost]
		public IActionResult GenerateLogs()
		{
			List<GenerateLogsVariables> logList = new() { };

			Array values = Enum.GetValues(typeof(Logging.LogLevel));
			const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

			int runs = Int32.Parse(Request.Form["amount"].ToString());

			for (int i = 0; i < runs; i++)
			{
				Logging.LogLevel randomLogType = (Logging.LogLevel)values.GetValue(_random.Next(values.Length));
				string error = new(Enumerable.Repeat(chars, 20).Select(s => s[_random.Next(s.Length)]).ToArray());
				logList.Add(new GenerateLogsVariables { LogType = randomLogType, Error = error });
			}

			List<string> data = new();

			Stopwatch sw = new();
			sw.Start();
			foreach (GenerateLogsVariables item in logList)
			{
				data.Add(item.Error);
				Logging.Log(Startup.Options, item.LogType, item.Error);
			}
			sw.Stop();
			data.Add($"Time it took for logging: {sw.Elapsed.TotalSeconds} seconds");

			TempData["postedLogs"] = data;

			return RedirectToAction("AddMultipleLogs", "Home");
		}

		public IActionResult Logs()
		{
			ViewData["wide"] = true;
			ViewData["html"] = System.IO.File.ReadAllText(Startup.LogFilePath);
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
