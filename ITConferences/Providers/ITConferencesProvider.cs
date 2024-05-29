using ITConferences.Enums;
using ITConferences.Helpers;
using ITConferences.Managers;
using ITConferences.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;

namespace ITConferences.Providers
{
	public class ITConferencesProvider
	{
		private ITConferencesManager _manager = new ITConferencesManager();
        private CountryManager _countryManager = new CountryManager();
        private TopicManager _topicManager = new TopicManager();

        public CrawlerStatus Crawler(Crawler crawler)
		{
            var result =  new CrawlerStatus();

			if (Enum.IsDefined(typeof(Crawler), crawler))
			{
                var Countries = _countryManager.GetAll();
                int countryCount = Countries.Count;

                var Topics = _topicManager.GetAll();
                int topicCount = Topics.Count;

                switch (crawler)
                {
                    case Enums.Crawler.DevEvents:
						int page = 1;
						int totalPage = 1;
                        int totalSuccess = 0;

                        while (page <= totalPage)
                        {
                            countryCount = Countries.Count;
                            topicCount = Topics.Count;

                            var res = ITConferecesCrawler.CrawlDevEvents(page, out int totalCount, ref Countries, ref Topics);

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

                                _manager.Add(res, page == 1, crawler);

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
                        var confereces = ITConferecesCrawler.CrawlPolytechnique(ref Countries, ref Topics);
                        if (confereces.Count > 0)
						{
                            _manager.Add(confereces, true, crawler);

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

		public bool Delete(Crawler type)
		{
			if (Enum.IsDefined(typeof(Crawler), type))
			{
                return _manager.Add(new List<ITConferenceModel>(), true, type);
			}
            else if ((int)type == 0)
            {
                var currents = _manager.GetAll();
                currents = currents.Where(t => t.crawler != 0).ToList();
				return _manager.Add(currents, true);
			}

			return false;
		}
		public List<ITConferenceModel> GetITConferences(string? query, int? countryId, string? type, int? topicId, out int totalCount, int start = 0, int length = 20)
		{
			var results = _manager.Get(query);

			if (results?.Count > 0)
			{
                results = results.OrderBy(t => t.startDate).ToList();

				if (countryId != null)
				{
                    results = results.Where(t => t.countryId == countryId).ToList();
                }

                if (!string.IsNullOrEmpty(type))
                {
                    if (type == "Other")
                    {
                        results = results.Where(t => string.IsNullOrEmpty(t.type)).ToList();
                    }
                    else
                    {
                        results = results.Where(t => t.type == type).ToList();
                    }
                }

                if (topicId != null)
                {
                    results = results.Where(t => t.topicId == topicId).ToList();
                }
            }

			if (results == null) results = new List<ITConferenceModel>();

			totalCount = results.Count;

			if (results.Count - start < length) length = results.Count - start;

            return results.GetRange(start, length);
        }

		public List<Aggregation> Aggregation(string? query, int? countryId, string? type, int? topicId)
		{
			var conferences = _manager.Get(query);

            if (conferences?.Count > 0)
            {
                if (countryId != null)
                {
                    conferences = conferences.Where(t => t.countryId == countryId).ToList();
                }

                if (!string.IsNullOrEmpty(type))
                {
                    if (type == "Other")
                    {
                        conferences = conferences.Where(t => string.IsNullOrEmpty(t.type)).ToList();
                    }
                    else
                    {
                        conferences = conferences.Where(t => t.type == type).ToList();
                    }
                }

                if (topicId != null)
                {
                    conferences = conferences.Where(t => t.topicId == topicId).ToList();
                }
            }

            if (conferences == null) conferences = new List<ITConferenceModel>();

            var Countries = _countryManager.GetAll();
            var Topics = _topicManager.GetAll();

            var types = conferences.GroupBy(t => t.type).ToDictionary(t => t.Key ?? "Other", t => t.Count());
            var countries = conferences.GroupBy(t => t.countryId).ToDictionary(t => t.Key, t => t.Count());
            var topics = conferences.GroupBy(t => t.topicId).ToDictionary(t => t.Key, t => t.Count());

            var aggregationTypes = new List<Aggregation>();
            var aggregationContries = new List<Aggregation>();
            var aggregationTopics = new List<Aggregation>();

            if (types?.Count > 0)
            {
                foreach (var item in types)
                {
                    aggregationTypes.Add(new Aggregation()
                    {
                        name = item.Key,
                        value = item.Key,
                        count = item.Value
                    });
                }
            }

            if (countries?.Count > 0)
            {
                foreach (var item in countries)
                {
                    var countryName = item.Key > 0 ? item.Key.ToString() : "Unknown country";
                    var country = Countries.Find(t => t.id == item.Key);
                    if (!string.IsNullOrEmpty(country?.countryName)) countryName = country.countryName;

                    aggregationContries.Add(new Aggregation()
                    {
                        name = countryName,
                        value = item.Key,
                        count = item.Value
                    });
                }
            }

            if (topics?.Count > 0)
            {
                foreach (var item in topics)
                {
                    var topicName = item.Key > 0 ? item.Key.ToString() : "Unknown topic";
                    var topic = Topics.Find(t => t.id == item.Key);
                    if (!string.IsNullOrEmpty(topic?.topicName)) topicName = topic.topicName;

                    aggregationTopics.Add(new Aggregation()
                    {
                        name = topicName,
                        value = item.Key,
                        count = item.Value
                    });
                }
            }

            return new List<Aggregation>()
            {
                new Aggregation("Type", "type", aggregationTypes),
                new Aggregation("Topic", "topicId", aggregationTopics),
                new Aggregation("Country", "countryId", aggregationContries),
            };
        }

        public List<AggregationSource> AggregationSource()
        {
            var aggregations = new List<AggregationSource>();
            var conferences = _manager.GetAll();

            var dictionary = conferences.GroupBy(t => t.crawler).ToDictionary(t => t.Key.ToString(), t => new AggregationCrawlwer(t.Count(), t.FirstOrDefault()?.crawlDate));

            foreach (Crawler source in Enum.GetValues(typeof(Crawler)))
            {
                AggregationCrawlwer? aggr = new AggregationCrawlwer(0, null);
                string link = string.Empty;

                if (ITConferecesCrawler.LinkSources.ContainsKey(source))
                {
                    link = ITConferecesCrawler.LinkSources[source];
				}

                if (dictionary.ContainsKey(source.ToString()))
                {
					aggr = dictionary[source.ToString()];
                }

                aggregations.Add(new AggregationSource(source.ToString(), source, link, aggr));
            }

            if (dictionary.ContainsKey("0"))
            {
                aggregations.Add(new AggregationSource("Other source", 0, null, dictionary["0"]));
            }

            return aggregations;
        }
    }
}
