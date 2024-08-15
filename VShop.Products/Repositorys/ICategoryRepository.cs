using VShop.Products.Models;

namespace VShop.Products.Repositorys
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetAll();
        Task<Category> GetById(int id);
        Task<IEnumerable<Category>> GetCategoryProducts();
        Task<Category> Create(Category category);
        Task<Category> Update(Category category);
        Task<Category> DeleteById(int id);
    }
}
