using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StoreApp9My.Models
{
    public class Product
    {
        public int Id { get; set; } // PRIMARY KEY
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public People? People { get; set; } // Navigation property to People
        [Required(ErrorMessage = "Owner is required")]
        public int PeopleId { get; set; } // Foreign key to People

        [NotMapped]
        public IFormFile? ImageFile { get; set; } // For file upload handling

        [NotMapped]
        public string? ImageLink { get;set; } // For displaying image link in the UI

    }
}
