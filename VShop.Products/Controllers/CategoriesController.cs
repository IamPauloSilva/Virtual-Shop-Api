using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VShop.Products.Dto;
using VShop.Products.Services.CategoryService;

namespace VShop.Products.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryInterface _categoryInterface;

        public CategoriesController(ICategoryInterface categoryInterface)
        {
            _categoryInterface = categoryInterface;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> Get()
        {
            var categoriesDto = await _categoryInterface.GetCategories();
            if (categoriesDto == null)
            {
                return NotFound("Categories not found");
            }
            return Ok(categoriesDto);
        }

        [HttpGet("products")]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategoriasProducts()
        {
            var categoriesDto = await _categoryInterface.GetCategoriesProducts();
            if (categoriesDto == null)
            {
                return NotFound("Categories not found");
            }
            return Ok(categoriesDto);
        }

        [HttpGet("{id:int}", Name = "GetCategory")]
        public async Task<ActionResult<CategoryDto>> Get(int id)
        {
            var categoryDto = await _categoryInterface.GetCategoryById(id);
            if (categoryDto == null)
            {
                return NotFound("Category not found");
            }
            return Ok(categoryDto);
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] CategoryDto categoryDto)
        {
            if (categoryDto == null)
                return BadRequest("Invalid Data");

            await _categoryInterface.AddCategory(categoryDto);

            return new CreatedAtRouteResult("GetCategory", new { id = categoryDto.CategoryId },
            categoryDto);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, [FromBody] CategoryDto categoryDto)
        {
            if (id != categoryDto.CategoryId)
                return BadRequest();

            if (categoryDto == null)
                return BadRequest();

            await _categoryInterface.UpdateCategory(categoryDto);

            return Ok(categoryDto);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<CategoryDto>> Delete(int id)
        {
            var categoryDto = await _categoryInterface.GetCategoryById(id);
            if (categoryDto == null)
            {
                return NotFound("Category not found");
            }

            await _categoryInterface.RemoveCategory(id);

            return Ok(categoryDto);
        }
    }
}
