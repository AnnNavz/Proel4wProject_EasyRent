using Microsoft.AspNetCore.Mvc;

namespace Proel4wProject_EasyRent.Controllers
{
	public class AdminController : Controller
	{
		public IActionResult Dashboard()
		{
			return View();
		}

		public IActionResult Logout()
		{
			HttpContext.Session.Clear();

			return RedirectToAction("LoginView", "Account");
    }
	}
}
