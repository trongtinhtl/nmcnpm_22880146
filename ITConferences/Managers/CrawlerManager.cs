using ITConferences.Enums;
using ITConferences.Models;

namespace ITConferences.Managers
{
	public class CrawlerManager
	{
		private StorageManager<CrawlerModel> _storage;

		public CrawlerManager()
		{
			_storage = new StorageManager<CrawlerModel> ("Crawlers.json");
		}

        public bool Add(CrawlerModel crawler)
        {
            if (crawler != null)
            {
                _storage.Add(crawler);
                return true;
            }
            return false;
        }

        public bool Update(CrawlerModel vm)
        {
            if (vm != null)
            {
                _storage.Update(t => t.id == vm.id, t => { t.crawler = vm.crawler; t.crawlerName = vm.crawlerName; t.crawlerUrl = vm.crawlerUrl; }) ;
                return true;
            }
            return false;
        }

        public bool Delete(int id)
        {
            if (id > 0)
            {
                _storage.Delete(t => t.id == id);
                return true;
            }
            return false;
        }

        public CrawlerModel? GetById(int id)
        {
            var crawlers = GetAll();

            if (crawlers?.Count > 0)
            {
                return crawlers.Find(t => t.id == id);
            }
            return null;
        }

        public int GetNextId()
        {
            int id = 0;
            var crawlers = GetAll();

            if (crawlers?.Count > 0)
            {
                var last = crawlers.OrderBy(t => t.id)?.LastOrDefault();
                if (last != null)
                {
                    id = last.id;
                }
            }

            return ++id;
        }

        public List<CrawlerModel> GetAll()
        { 
            return _storage.GetAll();
        }
    }
}
