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

        public CrawlerStatus Crawler(SourceType type)
		{
            var result =  new CrawlerStatus();

			if (Enum.IsDefined(typeof(SourceType), type))
			{
                switch (type)
                {
                    case SourceType.DevEvents:
						int page = 1;
						int totalPage = 1;
                        int totalSuccess = 0;

                        while (page <= totalPage)
                        {
                            var res = ITConferecesCrawler.CrawlDevEvents(page, out int totalCount);

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

                                _manager.Add(res, page == 1, type);

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
                    case SourceType.Polytechnique:
                        var confereces = ITConferecesCrawler.CrawlPolytechnique();
                        if (confereces.Count > 0)
						{
							_manager.Add(confereces, true, type);

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

		public bool Delete(SourceType type)
		{
			if (Enum.IsDefined(typeof(SourceType), type))
			{
                return _manager.Add(new List<ITConferenceModel>(), true, type);
			}
            else if ((int)type == 0)
            {
                var currents = _manager.GetAll();
                currents = currents.Where(t => t.source != 0).ToList();
				return _manager.Add(currents, true);
			}

			return false;
		}
		public List<ITConferenceModel> GetITConferences(string? query, string? country, string? type, string? tech, out int totalCount, int start = 0, int length = 20)
		{
			var results = _manager.Get(query);

			if (results?.Count > 0)
			{
				if (!string.IsNullOrEmpty(country))
				{
					if (country == "Other")
					{
                        results = results.Where(t => string.IsNullOrEmpty(t.country)).ToList();
                    }
					else
					{
						results = results.Where(t => t.country == country).ToList();
                    }
				}

                if (!string.IsNullOrEmpty(type))
                {
                    if (country == "Other")
                    {
                        results = results.Where(t => string.IsNullOrEmpty(t.type)).ToList();
                    }
                    else
                    {
                        results = results.Where(t => t.type == type).ToList();
                    }
                }

                if (!string.IsNullOrEmpty(tech))
                {
                    if (tech == "Other")
                    {
                        results = results.Where(t => string.IsNullOrEmpty(t.tech)).ToList();
                    }
                    else
                    {
                        results = results.Where(t => t.tech == tech).ToList();
                    }
                }
            }

			if (results == null) results = new List<ITConferenceModel>();

			totalCount = results.Count;

			if (results.Count - start < length) length = results.Count - start;

            return results.GetRange(start, length);
        }

		public List<Aggregation> Aggregation(string? query, string? country, string? type, string? tech)
		{
			var conferences = _manager.Get(query);

            if (conferences?.Count > 0)
            {
                if (!string.IsNullOrEmpty(country))
                {
                    if (country == "Other")
                    {
                        conferences = conferences.Where(t => string.IsNullOrEmpty(t.country)).ToList();
                    }
                    else
                    {
                        conferences = conferences.Where(t => t.country == country).ToList();
                    }
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

                if (!string.IsNullOrEmpty(tech))
                {
                    if (tech == "Other")
                    {
                        conferences = conferences.Where(t => string.IsNullOrEmpty(t.tech)).ToList();
                    }
                    else
                    {
                        conferences = conferences.Where(t => t.tech == tech).ToList();
                    }
                }
            }

            if (conferences == null) conferences = new List<ITConferenceModel>();

            var types = conferences.GroupBy(t => t.type).ToDictionary(t => t.Key ?? "Other", t => t.Count());
            var countries = conferences.GroupBy(t => t.country).ToDictionary(t => t.Key ?? "Other", t => t.Count());
            var techs = conferences.GroupBy(t => t.tech).ToDictionary(t => t.Key ?? "Other", t => t.Count());
            return new List<Aggregation>()
            {
                new Aggregation("Type", "type", types),
                new Aggregation("Tech/Languge", "tech", techs),
                new Aggregation("Country", "country", countries),
            };
        }

        public List<AggregationSource> AggregationSource()
        {
            var aggregations = new List<AggregationSource>();
            var conferences = _manager.GetAll();

            var dictionary = conferences.GroupBy(t => t.source).ToDictionary(t => t.Key.ToString(), t => new AggregationCrawlwer(t.Count(), t.FirstOrDefault()?.crawlDate));

            foreach (SourceType source in Enum.GetValues(typeof(SourceType)))
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
