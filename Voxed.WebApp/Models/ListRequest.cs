using System.Collections.Generic;

namespace Voxed.WebApp.Models
{
    public class ListRequest
    {
        public string Page { get; set; } //category-anm o home
        public int LoadMore { get; set; }
        public string Suscriptions { get; set; }
        public string Ignore { get; set; }
    }
}
