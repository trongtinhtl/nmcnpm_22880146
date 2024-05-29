namespace ITConferences.Models
{
    public class Aggregation
    {
        public string? name { get; set; }
        public object? value { get; set; }
        public object? aggregations { get; set; }

        public Aggregation() { }

        public Aggregation(string name, object? value, object aggregations)
        {
            this.name = name;
            this.value = value;
            this.aggregations = aggregations;
        }
    }

	public class AggregationSource
	{
		public string? name { get; set; }
		public object? value { get; set; }
		public string? link { get; set; }
		public AggregationCrawlwer? aggregations { get; set; }
		public AggregationSource() { }

		public AggregationSource(string name, object? value, string? link, AggregationCrawlwer aggregations)
		{
			this.name = name;
			this.value = value;
            this.link = link;
			this.aggregations = aggregations;
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
