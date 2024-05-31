using ITConferences.Enums;
using ITConferences.Managers;
using ITConferences.Models;
using System.Security.Cryptography;
using System.Text;

namespace ITConferences.Providers
{
    public class UserProvider
    {
        private UserManager _manager = new UserManager();

        public ApplicationUser? AddUser(ApplicationUser user, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (user != null && user.IsValid())
            {
                var current = GetUser(user.userName);

                if (current != null)
                {
                    errorMessage = "Username has exists!";
                    return null;
                }

                current = GetUserByEmail(user.email);

                if (current != null)
                {
                    errorMessage = "Email has exists!";
                    return null;
                }

                user.id = _manager.GetNextId();
                user.password = GetMd5Hash(user.password);

                if (_manager.Add(user))
                {
                    return user;
                }
            }
            else
            {
                errorMessage = "User invalid";
            }
            return null;
        }

        public ApplicationUser? UpdateUser(int id, Role role, bool blocked)
        {
            var current = GetUserById(id);

            if (current == null) throw new Exception("Account not found");

            current.role = role;
            current.blocked = blocked;

            var success = _manager.Update(current);

            return success ? current : null;
        }

        public bool DeleteUser(int id)
        {
            var current = GetUserById(id);

            if (current == null) throw new Exception("Account not found");

            return _manager.Delete(id);
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

        public ApplicationUser? GetUserById(int id)
        {
            var users = _manager.GetAll();

            if (users?.Count > 0)
            {
                return users.Find(t => t.id == id);
            }

            return null;
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

        public ApplicationUser? GetUserByEmail(string? email)
        {
            if (!string.IsNullOrEmpty(email))
            {
                var users = _manager.GetAll();

                if (users?.Count > 0)
                {
                    return users.Find(t => t.email == email);
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

        public static string? GetMd5Hash(string? input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                using (MD5 md5Hash = MD5.Create())
                {
                    byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

                    StringBuilder sBuilder = new StringBuilder();

                    for (int i = 0; i < data.Length; i++)
                    {
                        sBuilder.Append(data[i].ToString("x2"));
                    }

                    return sBuilder.ToString();
                }
            }

            return input;
        }
    }
}
