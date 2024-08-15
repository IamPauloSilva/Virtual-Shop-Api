using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using VShop.Web.Models;

namespace VShop.Web.Services.ProductService
{
    public class ProductService : IProductInterface
    {

        private readonly IHttpClientFactory _clientFactory;
        private readonly JsonSerializerOptions _options;
        private const string apiEndpoint = "/api/products/";
        private ProductModel _product;
        private IEnumerable<ProductModel> _products;

        public ProductService(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public async Task<IEnumerable<ProductModel>> GetAllProducts()
        {
            var client = _clientFactory.CreateClient("ProductApi");
            

            using (var response = await client.GetAsync(apiEndpoint))
            {
                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = await response.Content.ReadAsStreamAsync();
                    _products = await JsonSerializer
                                .DeserializeAsync<IEnumerable<ProductModel>>(apiResponse, _options);
                }
                else
                {
                    return null;
                }
            }
            return _products;
        }



        public async Task<ProductModel> FindProductById(int id)
        {
            var client = _clientFactory.CreateClient("ProductApi");
            

            using (var response = await client.GetAsync(apiEndpoint + id))
            {
                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = await response.Content.ReadAsStreamAsync();
                    _product = await JsonSerializer
                              .DeserializeAsync<ProductModel>(apiResponse, _options);
                }
                else
                {
                    
                    return null;
                }
            }
            return _product;
        }

        public async Task<ProductModel> CreateProduct(ProductModel product)
        {
            var client = _clientFactory.CreateClient("ProductApi");
            

            StringContent content = new StringContent(JsonSerializer.Serialize(product),
                                                      Encoding.UTF8, "application/json");

            using (var response = await client.PostAsync(apiEndpoint, content))
            {
                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = await response.Content.ReadAsStreamAsync();
                    product = await JsonSerializer
                               .DeserializeAsync<ProductModel>(apiResponse, _options);
                }
                else
                {
                    return null;
                    
                }
            }
            return product;
        }

        public async Task<ProductModel> UpdateProduct(ProductModel product)
        {
            var client = _clientFactory.CreateClient("ProductApi");
            

            ProductModel productUpdated = new ProductModel();

            using (var response = await client.PutAsJsonAsync(apiEndpoint, product))
            {
                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = await response.Content.ReadAsStreamAsync();
                    productUpdated = await JsonSerializer
                                      .DeserializeAsync<ProductModel>(apiResponse, _options);
                }
                else
                {
                    return null;
                    
                }
            }
            return productUpdated;
        }

        public async Task<bool> DeleteProductById(int id)
        {
            var client = _clientFactory.CreateClient("ProductApi");
            

            using (var response = await client.DeleteAsync(apiEndpoint + id))
            {
                if (response.IsSuccessStatusCode)
                {
                    
                    return true;
                }
            }
            return false;
        }

       
    }

}
