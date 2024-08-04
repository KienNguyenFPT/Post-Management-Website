using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;

namespace SignalRAssignment_ASM3.Models
{
    public class PostsManagementContext : DbContext
    {
        public PostsManagementContext(DbContextOptions<PostsManagementContext> options)
                : base(options)
        {
        }

        public DbSet<AppUser> Users { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<PostCategory> PostCategories { get; set; }

    }
}
