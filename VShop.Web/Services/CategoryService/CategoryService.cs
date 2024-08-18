using System.Net.Http.Headers;
using System.Text.Json;
using VShop.Web.Models;

namespace VShop.Web.Services.CategoryService
{
    public class CategoryService : ICategoryInterface
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly JsonSerializerOptions _options;
        private const string apiEndpoint = "/api/categories/";

        public CategoryService(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public async Task<IEnumerable<CategoryModel>> GetAllCategories(string token )
        {
            var client = _clientFactory.CreateClient("ProductApi");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            IEnumerable<CategoryModel> categories = new List<CategoryModel>(); // Default to empty list

            var response = await client.GetAsync(apiEndpoint);

            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadAsStreamAsync();
                categories = await JsonSerializer
                           .DeserializeAsync<IEnumerable<CategoryModel>>(apiResponse, _options);
            }
            else
            {
                // Log the error if needed (e.g. using ILogger)
                // return an empty list instead of null
                categories = new List<CategoryModel>(); // Ensure non-null return value
            }

            return categories;
        }
    }


}

