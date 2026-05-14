using Services.Dto.Response;

namespace Services.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<List<CategoryDto>> GetAllCategoriesAsync();
        Task CreateCategoryAsync(string name);
        Task DeleteCategoryAsync(int id);
    }
}
