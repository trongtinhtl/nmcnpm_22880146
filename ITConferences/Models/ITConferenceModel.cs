using ITConferences.Enums;

namespace ITConferences.Models
{
	public class ITConferenceModel
	{
		public SourceType source {  get; set; }
		public DateTime? crawlDate { get; set; }
		public string? name { get; set; }
		public string? description { get; set; }
		public string? type { get; set; }
        public string? tech { get; set; }
        public DateTime? startDate { get; set; }
		public DateTime? endDate { get; set; }
		public string? link { get; set; }
		public List<string>? images { get; set; }
		public string? performer { get; set; }
		public string? location { get; set; }
		public string? country { get; set; }
	}
}
