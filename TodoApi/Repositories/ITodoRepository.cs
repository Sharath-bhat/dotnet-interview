using TodoApi.Models;

namespace TodoApi.Repositories
{
    public interface ITodoRepository
    {
        Task<Todo> CreateAsync(Todo todo);
        Task<IEnumerable<Todo>> GetAllAsync();
        Task<Todo?> GetByIdAsync(int id);
        Task<Todo?> UpdateAsync(int id, Todo todo);
        Task<bool> DeleteAsync(int id);
    }
}