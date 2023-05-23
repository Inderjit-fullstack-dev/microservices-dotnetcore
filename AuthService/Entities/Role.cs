using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace AuthService.Entities
{
    public class Role
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public virtual ICollection<UserRole> UserRoles { get; set; }

        public Role()
        {
            UserRoles = new Collection<UserRole>();
        }
    }
}
