using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace AuthService.Entities
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        public string Password { get; set; } = string.Empty;

        [Required]
        public string PhoneNumber { get; set; } = string.Empty;
        public bool EmailConfirmed { get; set; } = false;
        public DateTime CreatedDate { get; set; }


        public virtual ICollection<UserRole> UserRoles { get; set; }

        public User()
        {
            UserRoles = new Collection<UserRole>();
        }
    }
}
