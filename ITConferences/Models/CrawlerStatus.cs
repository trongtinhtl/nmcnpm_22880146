namespace ITConferences.Models
{
	public class CrawlerStatus
	{
		public bool success { get; set; }
		public DateTime? lastModified { get; set; }
		public int total { get; set; }
		public string? error { get; set; }
	}
}
