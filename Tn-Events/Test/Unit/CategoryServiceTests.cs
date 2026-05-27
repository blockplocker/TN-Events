using Services.Services;

namespace Test.Unit
{
    public class CategoryServiceTests
    {

        [Fact]
        public async Task CategoryService_GetAllCategoriesAsync_ReturnsMappedCategories()
        {
            var repository = new CategoryRepositoryFake();
            repository.Categories.AddRange(new[]
            {
            ServiceTestData.Category(1, "Music"),
            ServiceTestData.Category(2, "Art")
        });
            var service = new CategoryService(repository);

            var result = await service.GetAllCategoriesAsync();

            Assert.Equal(2, result.Count);
            Assert.Collection(result,
                first => Assert.Equal("Music", first.Name),
                second => Assert.Equal("Art", second.Name));
        }

        [Fact]
        public async Task CategoryService_CreateCategoryAsync_AddsCategory()
        {
            var repository = new CategoryRepositoryFake();
            var service = new CategoryService(repository);

            await service.CreateCategoryAsync("Sports");

            Assert.Single(repository.Categories);
            Assert.NotNull(repository.LastCreated);
            Assert.Equal("Sports", repository.LastCreated!.Name);
        }

        [Fact]
        public async Task CategoryService_DeleteCategoryAsync_DelegatesToRepository()
        {
            var repository = new CategoryRepositoryFake();
            repository.Categories.Add(ServiceTestData.Category(4, "Sports"));
            var service = new CategoryService(repository);

            await service.DeleteCategoryAsync(4);

            Assert.Equal(4, repository.LastDeletedId);
            Assert.Empty(repository.Categories);
        }
    }
}
