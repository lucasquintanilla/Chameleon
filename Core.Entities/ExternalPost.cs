namespace Core.Entities
{
    public class ExternalPost : Post
    {
        public string ExternalId { get; set; }
        public string SourceId { get; set; }
    }
}
