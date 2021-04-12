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
    }
}
