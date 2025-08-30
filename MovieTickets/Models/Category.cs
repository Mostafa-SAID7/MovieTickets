namespace MovieTickets.Models
{
    public class Category
    {
        public int Id { get; set; }  // PK
        public string Name { get; set; }
        public ICollection<MovieCategory> MovieCategories { get; set; } = new List<MovieCategory>();
    }
}
