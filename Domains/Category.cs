using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace MyTasks_WebAPI.Domains
{
    public class Category
    {
        public Category()
        {
            Tasks = new Collection<Task>();
        }
        public int Id { get; set; }
        [MaxLength(50)]
        [Required(ErrorMessage = "Pole nazwa jest wymagane.")]
        [Display(Name = "Nazwa")]
        public string Name { get; set; }
        [Required]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public ICollection<Task> Tasks { get; set; }
    }
}
