using System.ComponentModel.DataAnnotations;

namespace SignalRAssignment_ASM3.Models
{
    public class AppUser
    {
        [Key]
        public int UserId { get; set; }
        public string FullName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        public string Address { get; set; }
        public Boolean Type { get; set; }

        public virtual ICollection<Post> Posts { get; set; }
    }
}
