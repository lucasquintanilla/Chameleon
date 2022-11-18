using System;

namespace Core.Entities
{
    public class Poll : Entity
    {
        public string OptionADescription { get; set; }
        public string OptionBDescription { get; set; }
        public int OptionAVotes { get; set; }
        public int OptionBVotes { get; set; }
    }

    public class PollOption
    {
        public string Description { get; set; }
        public int Votes { get; set; }
    }
}
