namespace ITConferences.Models
{
    public class ITConferenceRequest
    {
        public string? query { get; set; }
        public int? countryId { get; set; }
        public string? type { get; set; }
        public int? topicId { get; set; }
        public int start { get; set; }
        public int length { get; set; }
    }
}
