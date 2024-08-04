using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SignalRAssignment_ASM3.Models
{
    public class Post
    {
        [Key]
        public int PostId { get; set; }
        [Required]
        public int UserId { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string PublishStatus { get; set; }
        public int PostCategoryCategoryId { get; set; }
        [NotMapped]
        public string UserEmail { get; set; }
        [NotMapped]
        public string CategoryName { get; set; }

        public virtual AppUser User { get; set; }
        public virtual PostCategory PostCategory { get; set; }
    }
}
