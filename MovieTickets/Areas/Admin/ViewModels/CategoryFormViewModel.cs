namespace MovieTickets.Areas.Admin.ViewModels
{
    public class CategoryFormViewModel
    {
        public int? Id { get; set; }   // null for Create, not null for Edit
        public string Name { get; set; }

        // For concurrency (optional, but recommended if you add RowVersion in model later)
        public string? RowVersionBase64 { get; set; }
        public byte[]? RowVersion { get; set; }
    }
}
