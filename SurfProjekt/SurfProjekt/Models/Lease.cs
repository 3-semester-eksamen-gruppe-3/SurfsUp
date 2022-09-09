
namespace SurfProjekt.Models
{
    public class Lease
    {
        public int LeaseID { get; set; }
        public DateTime Date { get; set; }
        public double TimeFrame { get; set; }

        public int BoardID { get; set; }
        public Boards Board { get; set; }
        public int UserID { get; set; }        
    }
}
