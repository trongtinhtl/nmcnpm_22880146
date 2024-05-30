namespace ITConferences.Models
{
	public class CrawlerViewModel
	{
		public CrawlerModel? crawler { get; set; }
		public AggregationCrawlwer? aggregations { get; set; }
		public CrawlerViewModel() { }

		public CrawlerViewModel(CrawlerModel crawler, AggregationCrawlwer? aggregations = null)
		{
			this.crawler = crawler;

			if (aggregations != null)
			{
                this.aggregations = aggregations;
            }
			else
			{
				this.aggregations = new AggregationCrawlwer(0, null);
			}
		}
	}

	public class AggregationCrawlwer
	{
		public int count { get; set; }
		public DateTime? lastModified { get; set; }
		public AggregationCrawlwer() { }

		public AggregationCrawlwer(int count, DateTime? lastModified)
		{
			this.count = count;
			this.lastModified = lastModified;
		}
	}
}
