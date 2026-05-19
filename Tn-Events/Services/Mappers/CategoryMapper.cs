using DAL.Models;
using Services.Dto.Response;

namespace Services.Mappers
{
    public static class CategoryMapper
    {
        public static CategoryDto ToDto(Category category)
        {
            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name
            };
        }

        public static List<CategoryDto> ToDtoList(List<Category> categories)
        {
            return categories.Select(ToDto).ToList();
        }
    }
}
