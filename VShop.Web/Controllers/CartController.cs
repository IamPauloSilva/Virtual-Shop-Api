using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VShop.Web.Models;
using VShop.Web.Services;
using VShop.Web.Services.CartService;


namespace VShop.Web.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartInterface _cartInterface;
        

        public CartController(ICartInterface cartInterface)
        {
            _cartInterface = cartInterface;
            
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            CartViewModel? cartVM = await GetCartByUser();

            if(cartVM is null)
            {
                ModelState.AddModelError("CartNotFound", "Your Cart is Empty...");
                return View("/Views/Cart/CartNotFound.cshtml");
            }

            return View(cartVM);
        }



        private async Task<CartViewModel?> GetCartByUser()
        {

            var cart = await _cartInterface.GetCartByUserIdAsync(GetUserId(), await GetAccessToken());

            if(cart?.CartHeader is not null)
            {
                
                foreach (var item in cart.CartItems)
                {
                    cart.CartHeader.TotalAmount += (item.Product.Price * item.Quantity);
                }
                
            }
            return cart;
        }

        public async Task<IActionResult> RemoveItem(int id)
        {
            var result = await _cartInterface.RemoveItemFromCartAsync(id, await GetAccessToken());

            if (result)
            {
                return RedirectToAction(nameof(Index));
            }
            return View(id);
        }

        private async Task<string> GetAccessToken()
        {
            return await HttpContext.GetTokenAsync("access_token");
        }

        private string GetUserId()
        {
            return User.Claims.Where(u => u.Type == "sub")?.FirstOrDefault()?.Value;
        }
    }
}
