using Ecommerce;
using Ecommerce.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Ecommerce.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTest
    {
        HomeController addToCartTest = new HomeController();
        [TestMethod]
        public void ValidateStocksAndQuantity()
        {
            Assert.IsTrue(addToCartTest.ValidateStocksAndQuantity(10, 12));
            Assert.IsTrue(addToCartTest.ValidateStocksAndQuantity(8, 9));
            Assert.IsTrue(addToCartTest.ValidateStocksAndQuantity(3, 9));
            Assert.IsTrue(addToCartTest.ValidateStocksAndQuantity(1,2));
 
            Assert.IsFalse(addToCartTest.ValidateStocksAndQuantity(11,2));
            Assert.IsFalse(addToCartTest.ValidateStocksAndQuantity(16,5));
            Assert.IsFalse(addToCartTest.ValidateStocksAndQuantity(20,19));
            Assert.IsFalse(addToCartTest.ValidateStocksAndQuantity(1,0));
        }


        [TestMethod]
        public void ValidateCart()
        {
            

            int[] test1 = { 0, 0, 3};
            int[] test2 = { 0, 1, 1 };
            int[] test3 = { 1, 0, 1 };
            int[] test4 = { 1, 1, 1 };

            Assert.IsFalse(addToCartTest.ValidateCart(test1));
            Assert.IsFalse(addToCartTest.ValidateCart(test2));
            Assert.IsFalse(addToCartTest.ValidateCart(test3));
            Assert.IsTrue(addToCartTest.ValidateCart(test4));

        }

        //[TestMethod]
        //public void Index()
        //{
        //    // Arrange
        //    HomeController controller = new HomeController();

        //    // Act
        //    ViewResult result = controller.Index() as ViewResult;

        //    // Assert
        //    Assert.IsNotNull(result);
        //}

        //[TestMethod]
        //public void About()
        //{
        //    // Arrange
        //    HomeController controller = new HomeController();

        //    // Act
        //    ViewResult result = controller.About() as ViewResult;

        //    // Assert
        //    Assert.AreEqual("Your application description page.", result.ViewBag.Message);
        //}

        //[TestMethod]
        //public void Contact()
        //{
        //    // Arrange
        //    HomeController controller = new HomeController();

        //    // Act
        //    ViewResult result = controller.Contact() as ViewResult;

        //    // Assert
        //    Assert.IsNotNull(result);
        //}

         
    }
}
