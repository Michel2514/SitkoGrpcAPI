using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SitkoGrpcAPI.Data
{
    public class TodoItem
    {
        public Guid Id { get; set; }
        [Required] public string Name { get; set; }
        [Required] public DateTime CreationDate { get; set; } = DateTime.UtcNow;
        public DateTime? ExecutionDate { get; set; }
        public bool Completed { get; set; }
        public string? Description { get; set; }
    }
}