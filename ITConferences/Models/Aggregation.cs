namespace ITConferences.Models
{
    public class Aggregation
    {
        public string? name { get; set; }
        public object? value { get; set; }
		public int count { get; set; }
        public List<Aggregation>? aggregations { get; set; }

        public Aggregation() { }

        public Aggregation(string name, object? value, List<Aggregation>? aggregations)
        {
            this.name = name;
            this.value = value;
            this.aggregations = aggregations;
        }
    }
}
