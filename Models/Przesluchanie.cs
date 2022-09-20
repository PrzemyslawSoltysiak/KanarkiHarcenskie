using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace KanarkiHarcenskie.Models
{
    public class Przesluchanie
    {
        [Key]
        public int ID { get; set; }

        [Display(Name = "Identyfikator Kolekcji")]
        public int? ID_Kolekcji { get; set; }
        [ForeignKey("ID_Kolekcji")]
        public Kolekcja? PrzesluchiwanaKolekcja { get; set; }

        [DataType(DataType.Date)]
        public DateTime? Data { get; set; }

        [DataType(DataType.Time)]
        public DateTime? GodzinaOd { get; set; }

        [DataType(DataType.Time)]
        public DateTime? GodzinaDo { get; set; }
    }
}
