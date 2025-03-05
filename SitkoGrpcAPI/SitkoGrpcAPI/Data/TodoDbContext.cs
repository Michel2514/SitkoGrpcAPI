using Microsoft.EntityFrameworkCore;

namespace SitkoGrpcAPI.Data
{
    public class TodoDbContext : DbContext
    {
        public DbSet<TodoItem> TodoItems { get; set; }

        public TodoDbContext(DbContextOptions<TodoDbContext> options)
            : base(options)
        {
        }
    }
}