using ITConferences.Enums;
using ITConferences.Models;

namespace ITConferences.Managers
{
	public class CountryManager
	{
		private StorageManager<CountryModel> _storage;

		public CountryManager()
		{
			_storage = new StorageManager<CountryModel>("Countries.json");
		}

        public bool Add(CountryModel country)
        {
            if (country != null)
            {
                _storage.Add(country);
                return true;
            }
            return false;
        }

        public bool Save(List<CountryModel> countries)
        {
            if (countries?.Count > 0)
            {
                _storage.SaveAll(countries);
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

        public CountryModel? GetById(int id)
        {
            var countries = GetAll();

            if (countries?.Count > 0)
            {
                return countries.Find(t => t.id == id);
            }

            return null;
        }

        public CountryModel? GetByName(string name)
        {
            var countries = GetAll();

            if (countries?.Count > 0)
            {
                return countries.Find(t => t.countryName == name);
            }

            return null;
        }

        public List<CountryModel> GetAll()
        { 
            return _storage.GetAll();
        }
    }
}
