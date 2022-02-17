namespace Core.Entities
{
    public interface IMediaEntity
    {
        string Hash { get; set; }
        Media Media { get; set; }
    }
}
