using CourseManagementSystem.Core.Interfaces;
using CourseManagementSystem.Core.Models.Entities;

namespace CourseManagementSystem.Infrastructure.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _repo;

        public CategoryService(ICategoryRepository repo)
        {
            _repo = repo;
        }

        public Task<IEnumerable<Category>> GetAllAsync() => _repo.GetAllAsync();
        public Task<Category> GetByIdAsync(int id) => _repo.GetByIdAsync(id);
        public Task<Category> AddAsync(Category category) => _repo.AddAsync(category);
        public Task<Category> UpdateAsync(Category category) => _repo.UpdateAsync(category);
        public Task<bool> DeleteAsync(int id) => _repo.DeleteAsync(id);
    }
}