using VShop.Web.Models;

namespace VShop.Web.Services.ProductService
{
    public interface IProductInterface
    {
        Task<IEnumerable<ProductModel>> GetAllProducts(string token);
        Task<ProductModel> FindProductById(int id, string token);
        Task<ProductModel> CreateProduct(ProductModel product, string token);
        Task<ProductModel> UpdateProduct(ProductModel product, string token);
        Task<bool> DeleteProductById(int id, string token);
    }
}
