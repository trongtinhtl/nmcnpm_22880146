using ITConferences.Managers;
using ITConferences.Models;

namespace ITConferences.Providers
{
	public class ITConferencesProvider
	{
		private ITConferencesManager _manager = new ITConferencesManager();
        private CountryManager _countryManager = new CountryManager();
        private TopicManager _topicManager = new TopicManager();

		public bool Delete(int crawlerId)
		{
			if (crawlerId > 0)
			{
                return _manager.Add(new List<ITConferenceModel>(), true, crawlerId);
			}
            else if (crawlerId == 0)
            {
                var currents = _manager.GetAll();
                currents = currents.Where(t => t.crawlerId != 0).ToList();
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
    }
}
