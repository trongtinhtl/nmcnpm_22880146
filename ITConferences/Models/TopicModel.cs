namespace ITConferences.Models
{
    public class TopicModel
    {
        public int id { get; set; }
        public string? topicName { get; set; }
        public string? description { get; set; }

        public TopicModel() { }

        public TopicModel(int id, string? topicName)
        {
            this.id = id;
            this.topicName = topicName;
        }
    }
}
