using Microsoft.AspNetCore.Identity;
using System.Collections.ObjectModel;

namespace MyTasks_WebAPI.Domains
{
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
            Tasks = new Collection<Task>();
            Categories = new Collection<Category>();
        }
        public ICollection<Task> Tasks { get; set; }
        public ICollection<Category> Categories { get; set; }
    }
}
