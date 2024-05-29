using ITConferences.Enums;

namespace ITConferences.Models
{
    public class ApplicationUser
    {
        public string? userName { get; set; }
        public string? password { get; set; }
        public Role role { get; set; }

        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(userName) && !string.IsNullOrWhiteSpace(password);
        }
    }
}
