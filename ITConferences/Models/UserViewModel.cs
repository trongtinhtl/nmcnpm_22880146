using ITConferences.Enums;

namespace ITConferences.Models
{
    public class UserViewModel
    {
        public int id { get; set; }
        public string? userName { get; set; }
        public string? email { get; set; }
        public Role role { get; set; }
        public bool blocked { get; set; }

        public UserViewModel(ApplicationUser user)
        {
            if (user != null)
            {
                this.id = user.id;
                this.userName = user.userName;
                this.email = user.email;
                this.role = user.role;
                this.blocked = user.blocked;
            }
        }
    }
}
