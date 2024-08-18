using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using VShop.Products.Dto;
using VShop.Products.Roles;
using VShop.Products.Services.ProductService;

namespace VShop.Products.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class ProductsController : ControllerBase
    {
        private readonly IProductInterface _productInterface;

        public ProductsController(IProductInterface productInterface)
        {
            _productInterface = productInterface;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> Get()
        {
            var produtosDto = await _productInterface.GetProducts();
            if (produtosDto == null)
            {
                return NotFound("Products not found");
            }
            return Ok(produtosDto);
        }

        [HttpGet("{id}", Name = "GetProduct")]
        public async Task<ActionResult<ProductDto>> Get(int id)
        {
            var produtoDto = await _productInterface.GetProductById(id);
            if (produtoDto == null)
            {
                return NotFound("Product not found");
            }
            return Ok(produtoDto);
        }

        [HttpPost]
        [Authorize(Roles = Role.Admin)]
        public async Task<ActionResult> Post([FromBody] ProductDto produtoDto)
        {
            if (produtoDto == null)
                return BadRequest("Data Invalid");

            await _productInterface.AddProduct(produtoDto);

            return new CreatedAtRouteResult("GetProduct",
            new { id = produtoDto.Id }, produtoDto);
        }

        [HttpPut]
        [Authorize(Roles = Role.Admin)]
        public async Task<ActionResult<ProductDto>> Put([FromBody] ProductDto produtoDto)
        {
            if (produtoDto == null)
                return BadRequest("Data invalid");

            await _productInterface.UpdateProduct(produtoDto);
            return Ok(produtoDto);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = Role.Admin)]
        public async Task<ActionResult<ProductDto>> Delete(int id)
        {
            var produtoDto = await _productInterface.GetProductById(id);

            if (produtoDto == null)
            {
                return NotFound("Product not found");
            }

            await _productInterface.RemoveProduct(id);

            return Ok(produtoDto);
        }
    }
}