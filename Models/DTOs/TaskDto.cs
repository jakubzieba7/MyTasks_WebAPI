using MyTasks_WebAPI.Models.Domains;
using System.ComponentModel.DataAnnotations;

namespace MyTasks_WebAPI.Models.DTOs
{
    public class TaskDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }
        public bool IsExecuted { get; set; }
        public DateTime? Term { get; set; }
        public string UserId { get; set; }
    }
}
