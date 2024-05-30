using ITConferences.Enums;
using ITConferences.Models;
using ITConferences.Providers;
using Microsoft.AspNetCore.Mvc;

namespace ITConferences.Controllers
{
    [SessionAuthorize(Role.Admin)]
	public class AdminController : Controller
	{
		private ITConferencesProvider _conferenceProvider =  new ITConferencesProvider();
        private CrawlerProvider _crawlerProvider = new CrawlerProvider();
        public IActionResult Index()
		{
			return View();
		}

		[HttpPost]
		public JsonResult Crawler(int crawlerId)
		{
			try
			{
                var res = _crawlerProvider.Crawler(crawlerId);

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
		public JsonResult DeleteData(int crawlerId)
		{
			try
			{
				var res = _conferenceProvider.Delete(crawlerId);

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
        public JsonResult GetCrawler()
        {
            try
            {
                var res = _crawlerProvider.GetCrawler();

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
        public JsonResult AddCrawler(Crawler crawler, string crawlerName, string crawlerUrl)
        {
            try
            {
                var res = _crawlerProvider.AddCrawler(crawler, crawlerName, crawlerUrl);

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
        public JsonResult UpdateCrawler(int id, Crawler crawler, string crawlerName, string crawlerUrl)
        {
            try
            {
                var res = _crawlerProvider.UpdateCrawler(id, crawler, crawlerName, crawlerUrl);

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
        public JsonResult DeleteCrawler(int id)
        {
            try
            {
                var res = _crawlerProvider.DeleteCrawler(id);

                if (res)
                {
                    _conferenceProvider.Delete(id);
                }

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
