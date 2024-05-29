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
    }
}
