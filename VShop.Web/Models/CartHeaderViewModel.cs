using System.ComponentModel.DataAnnotations;

namespace VShop.Web.Models
{
    public class CartHeaderViewModel
    {
        public int Id { get; set; }

        
        public string UserId { get; set; } = string.Empty;

        public string? CouponCode { get; set; } = string.Empty;

        
        
        public decimal TotalAmount { get; set; } = 0.00m;

        
        public decimal Discount { get; set; } = 0.00m;

        [Required(ErrorMessage = "First Name is required.")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last Name is required.")]
        public string LastName { get; set; } = string.Empty;

        
        public DateTime DateTime { get; set; }

        
        public string Telephone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Card Number is required.")]
        
        public string CardNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Name on Card is required.")]
        public string NameOnCard { get; set; } = string.Empty;

        [Required(ErrorMessage = "CVV is required.")]
        public string CVV { get; set; } = string.Empty;

        [Required(ErrorMessage = "Expire Date is required.")]
        public string ExpireMonthYear { get; set; } = string.Empty;
    }
}
