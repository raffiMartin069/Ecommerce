using Antlr.Runtime.Tree;
using Ecommerce.Models.Cart;
using Ecommerce.Models.Store;
using Ecommerce.Repository.Cart;
using Ecommerce.Repository.Store;
using Ecommerce.Repository.User;
using Ecommerce.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace Ecommerce.Controllers
{
    //[Authorize]
    public class HomeController : Controller
    {

        public ActionResult Search(string search)
        {
            if (string.IsNullOrEmpty(search))
            {
                ViewData["NullSearch"] = "Search cannot be null";
                return RedirectToAction("Index");
            }

            try
            {
                ProductRepository prodRepo = new ProductRepository();
                List<ProductViewData> searchResults = prodRepo.FetchProductById(search);
                return View("Index", searchResults);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Error searching product: " + e.Message);
                ViewData["Error"] = "An error occurred while searching for the product.";
                return RedirectToAction("Index");
            }
        }

        public ActionResult Index()
        {
            try
            {
                ProductRepository productRepository = new ProductRepository();
                List<ProductViewData> prodList = productRepository.FetchAllProduct();
                return View(prodList);
            } catch(Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Exception caught while displaying contents: " + e);
                return View();
            }
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Checkout()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        private bool CartItem()
        {
            bool isAdded = true;




            return isAdded;
        }


        // Validate inputs from user add to cart.
        public bool ValidateCart(int[] a)
        {
            bool isValid = true;
            for(int i = 0; i < a.Length; i++)
            {
                if (a[i] == 0)
                {
                    isValid = false;
                }
            }
            return isValid;
        }

        public bool ValidateStocksAndQuantity(int quantity, int stocks)
        {
            return stocks >= quantity ? true : false;
        }

        [HttpGet]
        public ActionResult AddToCart(string id, string prodQty, string prodStock)
        {
            bool response = true;
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(prodQty) || string.IsNullOrEmpty(prodStock))
            {
                response = false;
                return Json(false, JsonRequestBehavior.AllowGet);
            }

            // This is the product id
            int productId = Convert.ToInt32(id);
            int quantity = Convert.ToInt32(prodQty);
            int productStocks = Convert.ToInt32(prodStock);

            int[] toCheck = { productId, quantity, productStocks };

            
            bool validation = ValidateCart(toCheck);

            if (!validation)
            {
                response = false;
                return Json(response, JsonRequestBehavior.AllowGet);
            }

            bool stockQuantityValidation = ValidateStocksAndQuantity(quantity, productStocks);

            if (!stockQuantityValidation)
            {
                response = false;
                return Json(response, JsonRequestBehavior.AllowGet);
            }

            // Get user id from session.
            string userId = (string)Session["userId"];

            try
            {
                // We create a cart for user.
                CartRepository cartRepo = new CartRepository();
                int cartId = cartRepo.CreateCart(userId);
                cartRepo.CartItemInsert(cartId, productId, quantity);

            } catch(Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Exception caught: " + e);
                response = false;
            }

            Session["cartId"] = productId;

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        private decimal GetSubTotal(List<UserCartViewData> userCart)
        {
            decimal totalPayment = 0;
            foreach (var bill in userCart)
            {
                decimal itemTotal = Convert.ToDecimal(bill.price.PP_PRICE) * Convert.ToInt32(bill.cartItem.CI_QTY);
                totalPayment += itemTotal;
                bill.TotalPrice = itemTotal;
            }
            return totalPayment;
        }

        private decimal GetTotal(decimal subtotal)
        {
            return Math.Round(subtotal * 1.12m, 2);
        }

        public ActionResult Cart()
        {
            try
            {
                CartRepository cartRepo = new CartRepository();
                UserRepository billing = new UserRepository();

                int userId = Convert.ToInt32(Session["userId"]);
                var userCart = cartRepo.UserCartDetails(userId);
                var bill = billing.BillingAddress(userId);
               

                if (userCart == null || userCart.Count < 1)
                {
                    return View();
                }

                if (bill == null || bill.Length < 6)
                {
                    return View();
                }

                ViewData["Street"] = bill[0];
                ViewData["Brgy"] = bill[1];
                ViewData["City"] = bill[2];
                ViewData["Province"] = bill[3];
                ViewData["Zip"] = bill[4];
                ViewData["Phone"] = bill[5];

                decimal subTotal = GetSubTotal(userCart);
                decimal total = GetTotal(subTotal);

                

                ViewData["SubTotalPayment"] = subTotal;
                ViewData["TotalPayment"] = total;

                return View(userCart);
            } catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Exception caught at Cart Action: " + e);
                return View();
            }

        }

        public ActionResult UpdateDeleteItem(string actionBtn, string qty, string cartItemId, string productId)
        {
            string action = actionBtn;
            int quantity = Convert.ToInt32(qty);
            int cartItem = Convert.ToInt32(cartItemId);
            int prodId = Convert.ToInt32(productId);

            if(prodId < 1)
            {
                return Json(new
                {
                    response = false,
                    mess = "Something went wrong. We'll get back later."
                });
            }

            if (quantity < 1)
            {
                if(action == "Update")
                {
                    return Json(new
                    {
                        response = false,
                        mess = "Quantity can not be lesser than 1. If you wish to remove click the Delete button below."
                    });
                }
            }

            CartRepository crud = new CartRepository();

            int productOriginalQty = crud.SearchProductQuantity(prodId);

            if(quantity > productOriginalQty)
            {
                if(action == "Update")
                {
                    return Json(new
                    {
                        response = false,
                        mess = "Entered quantity cannot be greater than the product listed quantity. Product listed quantity is " + productOriginalQty
                    });
                }
            }
            
            try
            {
                switch (action)
                {
                    case "Update":
                        bool update = crud.UpdateCartItemQuantity(cartItem, quantity);

                        if(!update)
                        {
                            return Json(new
                            {
                                response = false,
                                mess = "Failed updating the product"
                            });
                        }

                        return Json(new
                        {
                            response = true,
                            mess = "Item updated successfuly"
                        });

                    case "Delete":
                        bool delete = crud.DeleteCartItem(cartItem);
                        if (!delete)
                        {
                            return Json(new
                            {
                                response = false,
                                mess = "Failed deleting the product"
                            });
                        }

                        return Json(new
                        {
                            response = true,
                            mess = "Item deleted successfuly"
                        });

                    default:
                        break;
                }

                return RedirectToAction("Cart");
            } catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Exception caught: " + e);
                return RedirectToAction("Cart");
            }

        }

        public ActionResult TrackOrder()
        {
            return View();
        }
    }
}