using VShop.Web.Models;

namespace VShop.Web.Services.CartService;

public interface ICartInterface
{
    Task<CartViewModel> GetCartByUserIdAsync(string userId, string token);
    Task<CartViewModel> AddItemToCartAsync(CartViewModel cartVM, string token);
    Task<CartViewModel> UpdateCartAsync(CartViewModel cartVM, string token);
    Task<bool> RemoveItemFromCartAsync(int cartId, string token);

    
}
