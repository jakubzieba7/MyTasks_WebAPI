﻿using MyTasks_WebAPI.Models.Data;
using MyTasks_WebAPI.Models.Domains;

namespace MyTasks_WebAPI.Models.Repositories
{
    public class CategoryRepository
    {
        private ApplicationDbContext _context;

        public CategoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Category> GetCategories(string userId)
        {
            return _context.Categories.Where(x => x.UserId == userId).ToList();
        }

        public Category Get(int id, string userId)
        {
            return _context.Categories.Single(x => x.UserId == userId && x.Id == id);
        }

        public void Add(Category category)
        {
            _context.Categories.Add(category);
        }

        public void Update(Category category)
        {
            var categoryToUpdate = _context.Categories.Single(x => x.Id == category.Id && x.UserId == category.UserId);

            categoryToUpdate.Name = category.Name;
        }

        public void Delete(int id, string userId)
        {
            var categoryToDelete = _context.Categories.Single(x => x.Id == id && x.UserId == userId);

            _context.Categories.Remove(categoryToDelete);
        }

        public void AddDefaultCategory(string userId)
        {
            bool IsAnyCategoryExist = _context.Categories.Where(x => x.UserId == userId).Any();
            Category defaultCategory = new Category()
            {
                UserId = userId,
                Name = "Default",
            };

            if (!IsAnyCategoryExist)
                _context.Categories.Add(defaultCategory);
        }
    }
}
