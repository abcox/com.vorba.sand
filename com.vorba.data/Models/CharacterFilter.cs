namespace com.vorba.sand.services2.CosmosDb
{
    public class CharacterFilter
    {
        public string? Name { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? CreatedOnOrAfter { get; set; }
        public DateTime? CreatedOnOrBefore { get; set; }
        public DateTime? CreatedAfter { get; set; }
        public DateTime? CreatedBefore { get; set; }
    }
}