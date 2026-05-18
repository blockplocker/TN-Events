using DAL.Models;
using DAL.Repositories.Interfaces;
using Services.Dto.Response;
using Services.Services.Interfaces;

namespace Services.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<List<CategoryDto>> GetAllCategoriesAsync()
        {
            var categories = await _categoryRepository.GetAllAsync();
            return categories.Select(c => new CategoryDto { Id = c.Id, Name = c.Name }).ToList();
        }

        public async Task CreateCategoryAsync(string name)
        {
            var category = new Category
            {
                Name = name
            };
            await _categoryRepository.CreateAsync(category);
        }

        public async Task DeleteCategoryAsync(int id)
        {
            await _categoryRepository.DeleteAsync(id);
        }
    }
}
