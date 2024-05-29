namespace ITConferences.Models
{
    public class CountryModel
    {
        public int id { get; set; }
        public string? countryName {  get; set; }
        public string? countryCode { get; set; }
        public string? image {  get; set; }

        public CountryModel() { }

        public CountryModel(int id, string? countryName)
        {
            this.id = id;
            this.countryName = countryName;
        }
    }
}
