using VShop.Web.Models;

namespace VShop.Web.Services.CouponService
{
    public interface ICouponInterface
    {
        Task<CouponViewModel> GetDiscountCoupon(string couponCode, string token);
    }
}
