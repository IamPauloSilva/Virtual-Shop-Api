namespace VShop.Web.Models;

public class CartItemViewModel
{
    public int Id { get; set; }
    public ProductModel? Product { get; set; }
    public int Quantity { get; set; } = 1;
    public int ProductId { get; set; }
    public int CartHeaderId { get; set; }
}
