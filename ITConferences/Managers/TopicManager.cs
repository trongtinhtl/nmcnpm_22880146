using ITConferences.Enums;
using ITConferences.Models;

namespace ITConferences.Managers
{
	public class TopicManager
	{
		private StorageManager<TopicModel> _storage;

		public TopicManager()
		{
			_storage = new StorageManager<TopicModel>("Topics.json");
		}

        public bool Add(TopicModel topic)
        {
            if (topic != null)
            {
                _storage.Add(topic);
                return true;
            }
            return false;
        }

        public bool Save(List<TopicModel> topics)
        {
            if (topics?.Count > 0)
            {
                _storage.SaveAll(topics);
                return true;
            }
            return false;
        }

        public int GetNextId()
        {
            int id = 0;
            var countries = GetAll();

            if (countries?.Count > 0)
            {
                var last = countries.OrderBy(t => t.id)?.LastOrDefault();
                if (last != null)
                {
                    id = last.id;
                }
            }

            return ++id;
        }

        public TopicModel? GetById(int id)
        {
            var topics = GetAll();

            if (topics?.Count > 0)
            {
                return topics.Find(t => t.id == id);
            }

            return null;
        }

        public TopicModel? GetByName(string name)
        {
            var topics = GetAll();

            if (topics?.Count > 0)
            {
                return topics.Find(t => t.topicName == name);
            }

            return null;
        }

        public List<TopicModel> GetAll()
        { 
            return _storage.GetAll();
        }
    }
}
