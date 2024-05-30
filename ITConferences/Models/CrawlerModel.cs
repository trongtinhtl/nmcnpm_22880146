using ITConferences.Enums;

namespace ITConferences.Models
{
    public class CrawlerModel
    {
        public int id { get; set; }
        public Crawler crawler { get; set; }
        public string? crawlerName { get; set; }
        public string? crawlerUrl { get; set; }
    }
}
