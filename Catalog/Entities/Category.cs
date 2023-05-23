using System.ComponentModel.DataAnnotations;

namespace Catalog.Entities
{
    public class Category
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public string CategoryName { get; set; } = string.Empty;
    }
}
