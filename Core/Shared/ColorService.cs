using Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Shared
{
    public class StyleService
    {
        public static CommentStyle GetRandomCommentStyle()
        {
            Array styles = Enum.GetValues(typeof(CommentStyle));
            
            Random random = new Random();

            CommentStyle style = (CommentStyle)styles.GetValue(random.Next(styles.Length));

            while (style == CommentStyle.White || style == CommentStyle.Black)
            {
                style = (CommentStyle)styles.GetValue(random.Next(styles.Length));
            }

            return style;
        }
    }
}
