using Microsoft.EntityFrameworkCore;
using MyTasks_WebAPI.Models.Data;
using MyTasks_WebAPI.Models.Domains;
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
                            .Where(x => x.UserId == userId)
                            .OrderBy(x => x.Term)
                            .Skip((paginationFilter.PageNumber - 1) * paginationFilter.PageSize)
                            .Take(paginationFilter.PageSize)
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

        public Category CategoryVerification(Task task)
        {
            var isCategoryIdExist = _context.Categories.First(x => x.Id == task.CategoryId && x.UserId == task.UserId);

            return isCategoryIdExist;
        }
    }
}
