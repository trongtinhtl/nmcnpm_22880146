using ITConferences.Enums;
using ITConferences.Providers;
using Microsoft.AspNetCore.Mvc;

namespace ITConferences.Controllers
{
	public class AdminController : Controller
	{
		private ITConferencesProvider _provider =  new ITConferencesProvider();

		public IActionResult Index()
		{
			return View();
		}

		[HttpPost]
		public JsonResult Crawler(SourceType type)
		{
			try
			{
                var res = _provider.Crawler(type);

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

		[HttpPost]
		public JsonResult Delete(SourceType type)
		{
			try
			{
				var res = _provider.Delete(type);

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

		[HttpGet]
        public JsonResult AggregationSource()
        {
            try
            {
                var res = _provider.AggregationSource();

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
