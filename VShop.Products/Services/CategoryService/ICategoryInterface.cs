using VShop.Products.Dto;
using VShop.Products.Models;

namespace VShop.Products.Services.CategoryService
{
    public interface ICategoryInterface
    {
        Task<IEnumerable<CategoryDto>> GetCategories();
        Task<IEnumerable<CategoryDto>> GetCategoriesProducts();
        Task<CategoryDto> GetCategoryById(int id);
        Task AddCategory(CategoryDto categoryDto);
        Task UpdateCategory(CategoryDto categoryDto);
        Task RemoveCategory(int id);
    }
}
