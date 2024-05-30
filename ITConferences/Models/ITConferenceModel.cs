using ITConferences.Enums;

namespace ITConferences.Models
{
	public class ITConferenceModel
	{
		public int crawlerId {  get; set; }
		public DateTime? crawlDate { get; set; }
		public string? conferenceName { get; set; }
		public string? description { get; set; }
		public string? type { get; set; }
        public DateTime? startDate { get; set; }
		public DateTime? endDate { get; set; }
		public string? link { get; set; }
		public List<string>? images { get; set; }
		public string? performer { get; set; }
		public string? location { get; set; }
		public int countryId { get; set; }
        public int topicId { get; set; }
    }
}
