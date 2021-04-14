using Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Shared
{
    public class ColorService
    {
       
        public static string GetColor()
        {
            Random random = new Random();
            var list = new List<string> { "red", "blue", "green", "yellow", "multi" };
            int index = random.Next(list.Count);
            return list[index];
        }

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
