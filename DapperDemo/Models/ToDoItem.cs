using System.ComponentModel.DataAnnotations;

namespace DapperDemo.Models
{
    public class ToDoItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Title { get; set; }

        public bool Done { get; set; }
    }
}
