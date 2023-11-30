using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace BookStoreApp.Models
{
    public class Book
    {
        [Key]
        public string ISBN { get; set; }

        public int BookId { get; set; }  
        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        public double Price { get; set; }

        public int AuthorId { get; set; } = 0;

        public Author authorObject { get; set; } = null!;

        public int GenreId { get; set; } = 0;

        public Genre Genre { get; set; } = null!;

    }
}
