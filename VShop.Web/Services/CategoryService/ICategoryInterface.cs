using VShop.Web.Models;

namespace VShop.Web.Services.CategoryService
{
    public interface ICategoryInterface
    {
        Task<IEnumerable<CategoryModel>> GetAllCategories(string token);
    }
}
