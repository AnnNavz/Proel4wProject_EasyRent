using System.ComponentModel.DataAnnotations;

namespace Proel4wProject_EasyRent.Models
{
    public class Role
    {
        [Key]
        public int RoleId { get; set; }

        [Required(ErrorMessage ="User Role is required.")]
        [StringLength(50, MinimumLength = 4, ErrorMessage ="Must be between 4 and 50 characters.")]
        public string UserRole {  get; set; }
    }
}
