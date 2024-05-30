using ITConferences.Enums;
using ITConferences.Helpers;
using ITConferences.Managers;
using ITConferences.Models;

namespace ITConferences.Providers
{
    public class CrawlerProvider
    {
        private CrawlerManager _manager = new CrawlerManager();
        private ITConferencesManager _conferencesManager = new ITConferencesManager();
		private CountryManager _countryManager = new CountryManager();
		private TopicManager _topicManager = new TopicManager();

		public CrawlerStatus Crawler(int crawlerId)
		{
			var result = new CrawlerStatus();

			var crawler = _manager.GetById(crawlerId);

			if (crawler != null && !string.IsNullOrEmpty(crawler.crawlerUrl))
			{
				var Countries = _countryManager.GetAll();
				int countryCount = Countries.Count;

				var Topics = _topicManager.GetAll();
				int topicCount = Topics.Count;

				switch (crawler.crawler)
				{
					case Enums.Crawler.DevEvents:
						int page = 1;
						int totalPage = 1;
						int totalSuccess = 0;

						while (page <= totalPage)
						{
							countryCount = Countries.Count;
							topicCount = Topics.Count;

							var res = ITConferecesCrawler.CrawlDevEvents(crawler, page, out int totalCount, ref Countries, ref Topics);

							if (res.Count > 0)
							{
								if (page == 1)
								{
									if (totalCount > 0)
									{
										totalPage = totalCount / res.Count;
										if (totalCount % res.Count != 0) totalPage++;
									}
								}

								_conferencesManager.Add(res, page == 1, crawlerId);

								if (Countries.Count != countryCount)
								{
									_countryManager.Save(Countries);
								}

								if (Topics.Count != topicCount)
								{
									_topicManager.Save(Topics);
								}

								totalSuccess += res.Count;
							}

							if (page == totalPage)
							{
								result.lastModified = res.LastOrDefault()?.crawlDate;
							}
							page++;
						}

						result.success = totalSuccess > 0;
						result.total = totalSuccess;
						break;
					case Enums.Crawler.Polytechnique:
						var confereces = ITConferecesCrawler.CrawlPolytechnique(crawler, ref Countries, ref Topics);
						if (confereces.Count > 0)
						{
							_conferencesManager.Add(confereces, true, crawlerId);

							if (Countries.Count != countryCount)
							{
								_countryManager.Save(Countries);
							}

							if (Topics.Count != topicCount)
							{
								_topicManager.Save(Topics);
							}

							result.lastModified = confereces.LastOrDefault()?.crawlDate;
							result.success = true;
							result.total = confereces.Count;
						}
						break;
					default:
						break;
				}
			}
			else
			{
				result.error = "Type not valid!";
			}


			return result;
		}

		public CrawlerModel? AddCrawler(Crawler crawler, string crawlerName, string crawlerUrl)
        {
            if (Enum.IsDefined(typeof(Crawler), crawler) == false) throw new Exception("Crawler invalid");

            if (string.IsNullOrEmpty(crawlerName)) throw new Exception("Name invalid");

            if (string.IsNullOrEmpty(crawlerUrl)) throw new Exception("Url invalid");

            var vmCrawler = new CrawlerModel()
            {
                id = _manager.GetNextId(),
                crawler = crawler,
                crawlerName = crawlerName,
                crawlerUrl = crawlerUrl
            };

            var success = _manager.Add(vmCrawler);

			return success ? vmCrawler : null;
        }

        public CrawlerModel? UpdateCrawler(int id, Crawler crawler, string crawlerName, string crawlerUrl)
        {
            if (Enum.IsDefined(typeof(Crawler), crawler) == false) throw new Exception("Crawler invalid");

            if (string.IsNullOrEmpty(crawlerName)) throw new Exception("Name invalid");

            if (string.IsNullOrEmpty(crawlerUrl)) throw new Exception("Url invalid");

            var current = _manager.GetById(id);

            if (current == null) throw new Exception("Crawler not found");

            current.crawler = crawler;
            current.crawlerName = crawlerName;
            current.crawlerUrl = crawlerUrl;

            var success  = _manager.Update(current);

			return success ? current : null;
        }

        public bool DeleteCrawler(int id)
        {
            var current = _manager.GetById(id);

            if (current == null) throw new Exception("Crawler not found");

            return _manager.Delete(id);
        }

		public List<CrawlerViewModel> GetCrawler()
		{
			var results = new List<CrawlerViewModel>();

            var conferences = _conferencesManager.GetAll();
			var dictionary = conferences.GroupBy(t => t.crawlerId).ToDictionary(t => t.Key, t => new AggregationCrawlwer(t.Count(), t.FirstOrDefault()?.crawlDate));
			var crawlers = _manager.GetAll();

            if (crawlers?.Count > 0)
            {
                foreach ( var crawler in crawlers)
                {
					AggregationCrawlwer? aggr = new AggregationCrawlwer(0, null);

					if (dictionary.ContainsKey(crawler.id))
					{
						aggr = dictionary[crawler.id];

                        dictionary.Remove(crawler.id);
					}

					results.Add(new CrawlerViewModel(crawler, aggr));
				}
            }

			return results;
		}
	}
}
