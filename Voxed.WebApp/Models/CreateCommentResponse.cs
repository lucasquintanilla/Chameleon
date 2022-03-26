namespace Voxed.WebApp.Models
{
    public class CreateCommentResponse : BaseResponse
    {
        public CreateCommentResponse(string hash)
        {
            Hash = hash;
        }

        public string Hash { get; set; }
    }
}
