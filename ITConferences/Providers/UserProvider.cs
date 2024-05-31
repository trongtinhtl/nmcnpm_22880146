using ITConferences.Managers;
using ITConferences.Models;

namespace ITConferences.Providers
{
    public class UserProvider
    {
        private UserManager _manager = new UserManager();

        public bool AddUser(ApplicationUser user, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (user != null && user.IsValid())
            {
                var current = GetUser(user.userName);

                if (current == null)
                {
                    user.id = _manager.GetNextId();
                    _manager.Add(user);
                    return true;
                }
                else
                {
                    errorMessage = "Username has exists!";
                }
            }
            else
            {
                errorMessage = "User invalid";
            }
            return false;
        }

        public bool IsAdmin(string? userName)
        {
            var user = GetUser(userName);
            if (user != null)
            {
                return user.role == Enums.Role.Admin;
            }
            return false;
        }

        public ApplicationUser? GetUser(string? userName)
        {
            if (!string.IsNullOrEmpty(userName))
            {
                var users = _manager.GetAll();

                if (users?.Count > 0)
                {
                    return users.Find(t => t.userName == userName);
                }
            }

            return null;
        }

        public List<ApplicationUser>? GetUsers(string query)
        {
            var users = _manager.GetAll();

            if (!string.IsNullOrEmpty(query) && users?.Count > 0)
            {
                query = query.ToLower();

                users = users.Where(t => Contain(t.userName, query) || Contain(t.email, query)).ToList();
            }

            if (users == null) users = new List<ApplicationUser>();

            return users;
        }

        private bool Contain(string? value, string query)
        {
            if (!string.IsNullOrEmpty(value))
            {
                return value.ToLower().Contains(query);
            }

            return false;
        }
    }
}
