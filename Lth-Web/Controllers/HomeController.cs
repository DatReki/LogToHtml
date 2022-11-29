using LogToHtml;
using Lth_Web.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Lth_Web.Controllers
{
	public class HomeController : Controller
	{
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
			Log.Info(Startup.Options, $"Created new user: {username}");
			sw.Stop();

			List<string> data = new()
			{
				firstname,
				lastname,
				username,
				invalid,
				$"{Math.Round(sw.Elapsed.TotalSeconds, 3)} seconds"
			};
			sw.Reset();

			TempData["postedAccountForm"] = data;

			return RedirectToAction("Index", "Home");
		}

		public IActionResult Reset() => RedirectToAction("Index", "Home");

		public IActionResult AddMultipleLogs()
		{
			if (TempData["result"] != null)
				ViewData["result"] = TempData["result"];

			return View();
		}

		public class GenerateLogsVariables
		{
			public Log.LogLevel LogType { get; set; }
			public string Error { get; set; }
		}

		[HttpPost]
		public IActionResult GenerateLogs()
		{
			Stopwatch sw = new();
			int runs = int.Parse(Request.Form["amount"].ToString());
			sw.Start();
			int completed = Core.Functions.WriteRandomLogs(runs);
			sw.Stop();

			string result = $"It took {Math.Round(sw.Elapsed.TotalSeconds, 3)} seconds to write {completed} logs";
			TempData["result"] = result;

			return RedirectToAction("AddMultipleLogs", "Home");
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
