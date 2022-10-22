namespace Core.Entities
{
    public interface IHasAttachment
    {
        string Hash { get; set; }
        Attachment Attachment { get; set; }
    }
}
