using ITConferences.Enums;
using ITConferences.Models;

namespace ITConferences.Managers
{
	public class UserManager
	{
		private StorageManager<ApplicationUser> _storage;

		public UserManager()
		{
			_storage = new StorageManager<ApplicationUser> ("Users.json");
		}

        public bool Add(ApplicationUser user)
        {
            if (user != null)
            {
                _storage.Add(user);
                return true;
            }
            return false;
        }

        public List<ApplicationUser> GetAll()
        { 
            return _storage.GetAll();
        }

        public int GetNextId()
        {
            int id = 0;
            var users = GetAll();

            if (users?.Count > 0)
            {
                var last = users.OrderBy(t => t.id)?.LastOrDefault();
                if (last != null)
                {
                    id = last.id;
                }
            }

            return ++id;
        }

        public bool Update(ApplicationUser user)
        {
            if (user != null)
            {
                _storage.Update(t => t.id == user.id, t => { t.role = user.role; t.blocked = user.blocked; });
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
    }
}
