using ITConferences.Enums;

namespace ITConferences.Models
{
    public class ApplicationUser
    {
        public int id { get; set; }
        public string? userName { get; set; }
        public string? email { get; set; }
        public string? password { get; set; }
        public Role role { get; set; }
        public bool blocked { get; set; }

        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(userName) && !string.IsNullOrWhiteSpace(email) && !string.IsNullOrWhiteSpace(password);
        }
    }
}
