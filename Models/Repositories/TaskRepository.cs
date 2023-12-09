using Microsoft.EntityFrameworkCore;
using MyTasks_WebAPI.Models.Data;
using Task = MyTasks_WebAPI.Models.Domains.Task;

namespace MyTasks_WebAPI.Models.Repositories
{
    public class TaskRepository
    {
        private ApplicationDbContext _context;
        public TaskRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Task> Get(PaginationFilter paginationFilter, string userId)
        {
            return _context.Tasks
                            .OrderBy(x => x.Term)
                            .Skip((paginationFilter.PageNumber - 1) * paginationFilter.PageSize)
                            .Take(paginationFilter.PageSize)
                            .Where(x => x.UserId == userId)
                            .ToList();
        }

        public Task Get(int id, string userId)
        {
            return _context.Tasks.Single(x => x.Id == id && x.UserId == userId);
        }

        public void Add(Task task)
        {
            _context.Tasks.Add(task);
        }

        public void Update(Task task)
        {
            var taskToUpdate = _context.Tasks.Single(x => x.Id == task.Id && x.UserId == task.UserId);

            taskToUpdate.CategoryId = task.CategoryId;
            taskToUpdate.Description = task.Description;
            taskToUpdate.Title = task.Title;
            taskToUpdate.Term = task.Term;
            taskToUpdate.IsExecuted = task.IsExecuted;
        }

        public void Delete(int id, string userId)
        {
            var taskToDelete = _context.Tasks.Single(x => x.Id == id && x.UserId == userId);

            _context.Tasks.Remove(taskToDelete);
        }

        public void Finish(int id, string userId)
        {
            var taskToUpdate = _context.Tasks.Single(x => x.Id == id && x.UserId == userId);

            taskToUpdate.IsExecuted = true;
        }
    }
}
