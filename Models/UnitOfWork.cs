using MyTasks_WebAPI.Models.Data;
using MyTasks_WebAPI.Models.Repositories;

namespace MyTasks_WebAPI.Models
{
    public class UnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            TaskRepository = new TaskRepository(context);
            CategoryRepository = new CategoryRepository(context);
        }

        public CategoryRepository CategoryRepository { get; set; }
        public TaskRepository TaskRepository { get; set; }

        public void Complete()
        {
            _context.SaveChanges();
        }
    }
}
