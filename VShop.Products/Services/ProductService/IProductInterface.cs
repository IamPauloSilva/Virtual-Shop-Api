using VShop.Products.Dto;
using VShop.Products.Models;

namespace VShop.Products.Services.ProductService
{
    public interface IProductInterface
    {
        Task<IEnumerable<ProductDto>> GetProducts();
        Task<ProductDto> GetProductById(int id);
        Task AddProduct(ProductDto productDto);
        Task UpdateProduct(ProductDto productDto);
        Task RemoveProduct(int id);
    }
}
