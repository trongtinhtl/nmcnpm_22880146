﻿using ITConferences.Enums;
using ITConferences.Models;

namespace ITConferences.Managers
{
	public class ITConferencesManager
	{
		private StorageManager<ITConferenceModel> _storage;

		public ITConferencesManager()
		{
			_storage = new StorageManager<ITConferenceModel> ("ITConferences.json");
		}

        public bool Add(List<ITConferenceModel> conferences, bool replace = false, SourceType? type = null)
        {
            if (conferences != null)
            {
                if (replace)
                {
                    if (type != null && Enum.IsDefined(typeof(SourceType), type))
                    {
                        var currents = GetAll();
						currents = currents.Where(t => t.source != type).ToList();
                        currents.AddRange(conferences);

						_storage.SaveAll(currents);
					}
                    else
                    {
						_storage.SaveAll(conferences);
					}
                }
                else
                {
                    _storage.MultiAdd(conferences);
                }

                return true;
            }

            return false;
        }

        public List<ITConferenceModel> GetAll()
        { 
            return _storage.GetAll();
        }

        public List<ITConferenceModel> Get(string? query)
        {
            var conferences = GetAll();

            if (conferences?.Count > 0)
            {
                if (!string.IsNullOrEmpty(query))
                {
                    query = query.Trim().ToLower();

                    conferences = conferences.Where(t => t.name?.ToLower() == query ||
                                                          t.location?.ToLower() == query
                                                          ).ToList();
                }
            }

            return conferences ?? new List<ITConferenceModel>();
        }
    }
}