namespace MovieTickets.Models
{
    public class MovieActor
    {
        public int ActorId { get; set; }
        public int MovieId { get; set; }

        // Navigation properties
        public Actor Actor { get; set; }
        public Movie Movie { get; set; }
    }
}

