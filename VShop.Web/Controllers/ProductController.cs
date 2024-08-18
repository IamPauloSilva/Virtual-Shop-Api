using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;
using VShop.Web.Models;
using VShop.Web.Roles;
using VShop.Web.Services.CategoryService;
using VShop.Web.Services.ProductService;

namespace VShop.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductInterface _productInterface;
        private readonly ICategoryInterface _categoryInterface;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IProductInterface productService,
                                ICategoryInterface categoryService,
                                IWebHostEnvironment webHostEnvironment)
        {
            _productInterface = productService;
            _categoryInterface = categoryService;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductModel>>> Index()
        {

            var result = await _productInterface.GetAllProducts();

            if (result is null)
                return View("Error");

            return View(result);
        }

        [HttpGet]
        public async Task<IActionResult> CreateProduct()
        {
            ViewBag.CategoryId = new SelectList(await _categoryInterface.GetAllCategories(), "CategoryId", "Name");
            return View();
        }

        [HttpPost]
        [Authorize(Roles = Role.Admin)]
        public async Task<IActionResult> CreateProduct(ProductModel product)
        {
            if (ModelState.IsValid)
            {
                var result = await _productInterface.CreateProduct(product);

                if (result != null)
                    return RedirectToAction(nameof(Index));
            }
            else
            {
                ViewBag.CategoryId = new SelectList(await
                                     _categoryInterface.GetAllCategories(), "CategoryId", "Name");
            }
            return View(product);
        }

        [HttpGet]
        public async Task<IActionResult> UpdateProduct(int id)
        {
            ViewBag.CategoryId = new SelectList(await
                               _categoryInterface.GetAllCategories(), "CategoryId", "Name");

            var result = await _productInterface.FindProductById(id);

            if (result is null)
                return View("Error");

            return View(result);
        }

        [HttpPost]
        [Authorize(Roles = Role.Admin)]
        public async Task<IActionResult> UpdateProduct(ProductModel product)
        {
            if (ModelState.IsValid)
            {
                var result = await _productInterface.UpdateProduct(product);

                if (result is not null)
                    return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        [HttpGet]
        public async Task<ActionResult<ProductModel>> DeleteProduct(int id)
        {
            var result = await _productInterface.FindProductById(id);

            if (result is null)
                return View("Error");

            return View(result);
        }

        [HttpPost(), ActionName("DeleteProduct")]
        [Authorize(Roles = Role.Admin)]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _productInterface.DeleteProductById(id);

            if (!result)
                return View("Error");

            return RedirectToAction("Index");
        }
        private async Task<string> GetAccessToken()
        {
            return await HttpContext.GetTokenAsync("access_token");
        }
    }
}
