
namespace SurfProjekt.Models
{
    public class Lease
    {
        public int LeaseID { get; set; }
        public DateTime Date { get; set; }
        public int TimeFrame { get; set; }

        public int BoardID { get; set; }
        public Boards Board { get; set; }
        public string UserID { get; set; }
        
        public Lease()
        {

        }
        public Lease(int boardId, string userId, int timeFrame, DateTime date)
        {
            BoardID = boardId;
            UserID = userId;
            TimeFrame = timeFrame;
            Date = date;

        }
    }
}
