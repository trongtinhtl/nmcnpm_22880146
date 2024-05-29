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
    }
}
