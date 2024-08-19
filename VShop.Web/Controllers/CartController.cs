using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VShop.Web.Models;
using VShop.Web.Services;
using VShop.Web.Services.CartService;
using VShop.Web.Services.CouponService;


namespace VShop.Web.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartInterface _cartInterface;
        private readonly ICouponInterface _couponInterface;

        public CartController(ICartInterface cartInterface, ICouponInterface couponInterface)
        {
            _cartInterface = cartInterface;
            _couponInterface = couponInterface;
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

            if (cart?.CartHeader is not null)
            {
                if (!string.IsNullOrEmpty(cart.CartHeader.CouponCode))
                {
                    var coupon = await _couponInterface.GetDiscountCoupon(cart.CartHeader.CouponCode,
                                                                        await GetAccessToken());
                    if (coupon?.CouponCode is not null)
                    {
                        cart.CartHeader.Discount = coupon.Discount;
                    }
                }
                foreach (var item in cart.CartItems)
                {
                    cart.CartHeader.TotalAmount += (item.Product.Price * item.Quantity);
                }

                cart.CartHeader.TotalAmount = cart.CartHeader.TotalAmount -
                                             (cart.CartHeader.TotalAmount *
                                              cart.CartHeader.Discount) / 100;
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

        [HttpPost]
        public async Task<IActionResult> ApplyCoupon(CartViewModel cartVM)
        {
            if (ModelState.IsValid)
            {
                var result = await _cartInterface.ApplyCouponAsync(cartVM, await GetAccessToken());
                if (result)
                {
                    return RedirectToAction(nameof(Index));
                }
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCoupon()
        {
            var result = await _cartInterface.RemoveCouponAsync(GetUserId(), await GetAccessToken());

            if (result)
            {
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            CartViewModel? cartVM = await GetCartByUser();
            return View(cartVM);
        }

        [HttpPost]
        public async Task<IActionResult> Checkout(CartViewModel cartVM)
        {
            if (ModelState.IsValid)
            {
                var result = await _cartInterface.CheckoutAsync(cartVM.CartHeader, await GetAccessToken());

                if (result is not null)
                {
                    return RedirectToAction(nameof(CheckoutCompleted));
                }
            }
            return View(cartVM);
        }

        [HttpGet]
        public IActionResult CheckoutCompleted()
        {
            return View();
        }
    }
}
