using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Entities
{
    public interface IMediaEntity
    {
        string Hash { get; set; }
        Media Media { get; set; }
    }
}
