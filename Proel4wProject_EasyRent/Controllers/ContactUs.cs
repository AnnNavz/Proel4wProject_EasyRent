using Microsoft.AspNetCore.Mvc;

namespace Proel4wProject_EasyRent.Controllers
{
	public class ContactUs : Controller
	{
		public IActionResult ContactUsView()
		{
			return View();
		}
	}
}
