namespace ITConferences.Models
{
    public class ITConferenceRequest
    {
        public string? query { get; set; }
        public string? country { get; set; }
        public string? type { get; set; }
        public string? tech { get; set; }
        public int start { get; set; }
        public int length { get; set; }
    }
}
