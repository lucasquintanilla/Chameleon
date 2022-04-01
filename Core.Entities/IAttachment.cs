namespace Core.Entities
{
    public interface IAttachment
    {
        string Hash { get; set; }
        Media Media { get; set; }
    }
}
