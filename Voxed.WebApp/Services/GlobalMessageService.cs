using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Voxed.WebApp.Services
{
    public class GlobalMessageFormViewModel
    {
        public enum GlobalMessageType { TenMinutes, ThirtyMinutes, OneHour, TwoHours, FourHours, TwentyFourHours }

        [Required(ErrorMessage = "Debe ingresar un contenido")]
        [StringLength(140, ErrorMessage = "El contenido no puede superar los {1} caracteres.")]
        public string Content { get; set; }
        public GlobalMessageType Type { get; set; }        
    }

    public class GlobalMessage
    {
        public string Content { get; set; }
        public int Tokens { get; set; }
        public int Color { get; set; } // 50 = gris, 4000 = multi, 
        public DateTimeOffset CreatedOn { get; set; } = DateTimeOffset.Now;
        public DateTimeOffset DueDate { get; set; }
    }

    public static class GlobalMessageService
    {
        private static List<GlobalMessage> _messages = new List<GlobalMessage>();

        public static void AddMessage(GlobalMessage message)
        {
            _messages.Add(message);
        }

        public static List<GlobalMessage> GetActiveMessages()
        {
            return _messages.Where(x => x.DueDate > DateTimeOffset.Now).ToList();
        }
    }
}
