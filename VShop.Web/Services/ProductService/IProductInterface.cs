using VShop.Web.Models;

namespace VShop.Web.Services.ProductService
{
    public interface IProductInterface
    {
        Task<IEnumerable<ProductModel>> GetAllProducts();
        Task<ProductModel> FindProductById(int id);
        Task<ProductModel> CreateProduct(ProductModel productModel);
        Task<ProductModel> UpdateProduct(ProductModel productModel);
        Task<bool> DeleteProductById(int id);
    }
}
