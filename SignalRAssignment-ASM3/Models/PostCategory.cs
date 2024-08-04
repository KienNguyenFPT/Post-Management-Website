using System.ComponentModel.DataAnnotations;

namespace SignalRAssignment_ASM3.Models
{
    public class PostCategory
    {
        [Key]
        public int CategoryId { get; set; }
        [Required]
        public string CategoryName { get; set; }
        public string Description { get; set; }
        public virtual ICollection<Post> Posts { get; set; }
    }
}
