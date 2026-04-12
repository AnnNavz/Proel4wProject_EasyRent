using Microsoft.AspNetCore.Mvc;

namespace Proel4wProject_EasyRent.Controllers
{
	public class AboutUs : Controller
	{
		public IActionResult AboutUsView()
		{
			return View();
		}
	}
}
