using VShop.Web.Models;

public class CartViewModel
{
    public CartHeaderViewModel CartHeader { get; set; } = new CartHeaderViewModel();
    public ICollection<CartItemViewModel> CartItems { get; set; } = new List<CartItemViewModel>();
}
