using Ecommerce.Models;
using Ecommerce.Models.Cart;
using Ecommerce.Models.Store;
using Ecommerce.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ecommerce.ViewModel
{
    public class UserCartViewData
    {
        public Cart cart {  get; set; }
        public CartItem cartItem { get; set; }
        public UserModel user { get; set; }
        public Product product { get; set; }
        public ProductPrice price { get; set; }
        public ProductQuantity productQuantity { get; set; }
        public int productCount { get; set; }
        public decimal TotalPrice { get; set; }
    }
}