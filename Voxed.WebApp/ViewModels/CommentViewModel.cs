using System;

namespace Voxed.WebApp.ViewModels
{
    public class CommentViewModel
    {
        public Guid Id { get; set; }
        public string Hash { get; set; }
        public string Content { get; set; }
        public string AvatarColor { get; set; }
        public string AvatarText { get; set; }
        public MediaViewModel Media { get; set; }
        public bool IsOp { get; set; }

    }
}
