using System.ComponentModel.DataAnnotations;

namespace SurfProjekt.Models
{
    public class Boards
    {
        public int Id { get; set; }

        [Display(Name = "Navn")]
        public string Name { get; set; }

        [Display(Name = "Længde (feet)")]
        public double Length { get; set; }

        [Display(Name = "Bredde (inches)")]
        public double Width { get; set; }

        [Display(Name = "Tykkelse (inches)")]
        public double Thickness { get; set; }

        [Display(Name = "Volumen (L)")]
        public double Volume { get; set; }

        public string Type { get; set; }

        [Display(Name = "Pris (€)")]
        public double Price { get; set; }

        [Display(Name = "Udstyr")]
        public string? Equipment { get; set; }
    }
}
