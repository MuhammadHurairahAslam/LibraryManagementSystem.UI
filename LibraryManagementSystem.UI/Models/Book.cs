namespace LibraryManagementSystem.UI.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Author { get; set; }
        public string? ISBN { get; set; }
        public BookGenre Genre { get; set; }
        public BookFormat Format { get; set; }
        public BookAvailability Availability { get; set; }
        public DateTime PublishedDate { get; set; }
        public string? Description { get; set; }
    }
}
