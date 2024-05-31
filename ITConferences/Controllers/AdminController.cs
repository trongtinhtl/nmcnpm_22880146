using ITConferences.Enums;
using ITConferences.Models;
using ITConferences.Providers;
using Microsoft.AspNetCore.Mvc;

namespace ITConferences.Controllers
{
    //[SessionAuthorize(Role.Admin)]
	public class AdminController : Controller
	{
		private ITConferencesProvider _conferenceProvider =  new ITConferencesProvider();
        private CrawlerProvider _crawlerProvider = new CrawlerProvider();
        private UserProvider _userProvider = new UserProvider();
        public IActionResult Index()
		{
			return View();
		}

        public IActionResult UserManagement()
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

        [HttpPost]
        public JsonResult GetUsers(string query)
        {
            try
            {
                var res = _userProvider.GetUsers(query);

                return Json(new
                {
                    success = true,
                    value = res?.Select(t => new UserViewModel(t))
                }); ;
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
        public JsonResult AddUser(string userName, string email, string password, string confirmPassword, Role role, bool blocked)
        {
            try
            {
                if (string.IsNullOrEmpty(userName)) throw new Exception("Username empty");
                if (string.IsNullOrEmpty(email)) throw new Exception("Email empty");
                if (string.IsNullOrEmpty(password)) throw new Exception("Password empty");
                if (string.IsNullOrEmpty(confirmPassword)) throw new Exception("Confirm password empty");
                if (confirmPassword != password) throw new Exception("Confirm password not match");

                var user = new ApplicationUser()
                {
                    userName = userName,
                    email = email,
                    password = password,
                    role = role,
                    blocked = blocked
                };

                var res = _userProvider.AddUser(user, out string errorMessage);

                if (res != null)
                {
                    return Json(new
                    {
                        success = true,
                        value = new UserViewModel(res)
                    });
                }

                throw new Exception(errorMessage ?? "Create account not successfull");
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
        public JsonResult UpdateUser(int id, Role role, bool blocked)
        {
            try
            {
                var res = _userProvider.UpdateUser(id, role, blocked);

                return Json(new
                {
                    success = true,
                    value = res != null ? new UserViewModel(res) : null
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
        public JsonResult DeleteUser(int id)
        {
            try
            {
                var res = _userProvider.DeleteUser(id);

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
