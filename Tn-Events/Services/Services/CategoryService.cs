using DAL.Models;
using DAL.Repositories.Interfaces;
using Services.Dto.Response;
using Services.Mappers;
using Services.Services.Interfaces;

namespace Services.Services
{
    public class CategoryService(ICategoryRepository categoryRepository) : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository = categoryRepository;

        public async Task<List<CategoryDto>> GetAllCategoriesAsync()
        {
            var categories = await _categoryRepository.GetAllAsync();
            return CategoryMapper.ToDtoList(categories);
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
