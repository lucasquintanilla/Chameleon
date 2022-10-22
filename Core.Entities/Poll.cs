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

    //public class EncuestaViewModel
    //{
    //    public EncuestaViewModel(Encuesta encuesta, string usuarioId)
    //    {
    //        Opciones = encuesta.Opciones;
    //        HaVotado = encuesta.Ids.Any(id => id == usuarioId);
    //    }
    //    public IList<OpcionEncuesta> Opciones { get; set; }
    //    public bool HaVotado { get; set; } = false;
    //}
}
