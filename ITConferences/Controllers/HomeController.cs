using ITConferences.Models;
using ITConferences.Providers;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ITConferences.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private readonly ITConferencesProvider _provider;

		public HomeController(ILogger<HomeController> logger)
		{
			_logger = logger;
            _provider = new ITConferencesProvider();
        }

		public IActionResult Index()
		{
			return View();
		}

		public IActionResult Login()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}

		[HttpPost]
		public JsonResult GetITConferences(ITConferenceRequest request)
		{
			try
			{
				int totalCount = 0;
				List<ITConferenceModel> res = new List<ITConferenceModel>();

				if (request != null)
				{
					res = _provider.GetITConferences(request.query, request.countryId, request.type, request.topicId, out totalCount, request.start, request.length);
                }

				return Json(new
				{
					success = true,
					value = res,
                    totalCount
                });
			}
			catch (Exception ex)
			{
                return Json(new
                {
                    success = false,
					error = ex.Message
                });
            }
		}

        [HttpPost]
        public JsonResult Aggregation(ITConferenceRequest request)
        {
            try
            {
                var res = _provider.Aggregation(request?.query, request?.countryId, request?.type, request?.topicId);

                return Json(new
                {
                    success = true,
                    value = res
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    error = ex.Message
                });
            }
        }
    }
}