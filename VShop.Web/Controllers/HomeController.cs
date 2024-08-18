using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using VShop.Web.Models;
using VShop.Web.Services.ProductService;

namespace VShop.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductInterface _productInterface;

        public HomeController(ILogger<HomeController> logger,IProductInterface productInterface)
        {
            _logger = logger;
            _productInterface = productInterface;
        }

        public async Task<IActionResult> Index()
        {
            var result = await _productInterface.GetAllProducts(string.Empty);

            if (result is null)
                return View("Error");

            return View(result);
            
        }

        [HttpGet]
        
        public async Task<ActionResult<ProductModel>> ProductDetails(int id)
        {
            var token = await HttpContext.GetTokenAsync("access_token");
            var product = await _productInterface.FindProductById(id, token);

            if (product is null)
                return View("Error");

            return View(product);
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Authorize]
        public async Task<IActionResult> Login()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Logout()
        {
            return SignOut("Cookies", "oidc");
        }
    }
}
