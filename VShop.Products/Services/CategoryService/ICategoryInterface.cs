using VShop.Products.Models;

namespace VShop.Products.Services.CategoryService
{
    public interface ICategoryInterface
    {
        Task<IEnumerable<Category>> GetAll();
        Task<Category> GetById(int id);
        Task<IEnumerable<Category>> GetCategoryProducts();
        Task<Category> Create();
        Task<Category> Update();
        Task<Category> DeleteById(int id);
    }
}
