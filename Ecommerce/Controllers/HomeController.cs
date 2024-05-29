using Ecommerce.Models.Store;
using Ecommerce.Repository.Store;
using Ecommerce.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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

        public ActionResult Cart()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult TrackOrder()
        {
            return View();
        }
    }
}