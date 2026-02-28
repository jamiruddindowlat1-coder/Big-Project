using CourseManagementSystem.Core.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CourseManagementSystem.Core.Interfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<User> GetByIdAsync(int id);
        Task<User> GetByEmailAsync(string email);
        Task<User> AddAsync(User user); // Return type change: Task -> Task<User>
        Task UpdateAsync(User user);
        Task DeleteAsync(int id);
    }
}