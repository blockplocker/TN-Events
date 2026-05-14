using DAL.Models;

namespace DAL.Repositories.Interfaces
{
    public interface ICategoryRepository
    {
        Task<List<Category>> GetAllAsync();
        Task<Category> CreateAsync(Category category);
        Task DeleteAsync(int id);
    }
}
