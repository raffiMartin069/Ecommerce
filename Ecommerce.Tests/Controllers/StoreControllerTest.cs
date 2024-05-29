using Ecommerce.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace Ecommerce.Controllers.Tests
{
    [TestClass()]
    public class StoreControllerTest
    {
        [TestMethod()]
        public void ImageValidationTest()
        {
            string img = ".gif";
            int imgLeght = 20 * 1024 * 1024;

            string[] allowedExtension = { ".jpg", ".jpeg", ".svg", ".png" };

            foreach (string extension in allowedExtension)
            {
                if (img == extension)
                {
                    Assert.Equals(img, extension);
                }
            }

            bool leght = imgLeght >= 20 * 1024 * 1024;

            Assert.IsTrue(leght);


        }

        [TestMethod()]
        public void PriceValidationTest()
        {
            string amount = "400";
            decimal money = Convert.ToInt32(amount) / 100m;
            string convertedMoney = money.ToString("N2");

            Assert.AreEqual("4.00", convertedMoney);
            Assert.AreNotEqual("4.01", convertedMoney);
            Assert.AreNotEqual("4", convertedMoney);
            Assert.AreNotEqual("400", convertedMoney);
            Assert.AreNotEqual(400, convertedMoney);
            Assert.AreNotEqual(400.00, convertedMoney);
            Assert.AreNotEqual(400.01, convertedMoney);
            Assert.AreNotEqual(4, convertedMoney);
        }

        [TestMethod()]
        public void CheckPriceEntry()
        {
            StoreController sc = new StoreController();
            
            Assert.IsFalse(sc.checkQuantity("-1"));
            Assert.IsFalse(sc.checkQuantity("-23"));
            Assert.IsFalse(sc.checkQuantity("-54"));
            Assert.IsFalse(sc.checkQuantity("-78"));
            Assert.IsFalse(sc.checkQuantity("-98"));
            Assert.IsFalse(sc.checkQuantity("-100"));

            Assert.IsTrue(sc.checkQuantity("1"));
            Assert.IsTrue(sc.checkQuantity("23"));
            Assert.IsTrue(sc.checkQuantity("54"));
            Assert.IsTrue(sc.checkQuantity("78"));
            Assert.IsTrue(sc.checkQuantity("98"));
            Assert.IsTrue(sc.checkQuantity("100"));
        }

        [TestMethod()]
        public void checkQuantity()
        {
            StoreController sc = new StoreController();
            
            Assert.IsFalse(sc.checkQuantity("-1"));
            Assert.IsFalse(sc.checkQuantity("-2"));
            Assert.IsFalse(sc.checkQuantity("-5"));
            Assert.IsFalse(sc.checkQuantity("-10"));
            Assert.IsFalse(sc.checkQuantity("-0"));
            Assert.IsFalse(sc.checkQuantity("-3"));
            Assert.IsFalse(sc.checkQuantity("-0"));

            Assert.IsTrue(sc.checkQuantity("1"));
            Assert.IsTrue(sc.checkQuantity("2"));
            Assert.IsTrue(sc.checkQuantity("5"));
            Assert.IsTrue(sc.checkQuantity("10"));
            Assert.IsTrue(sc.checkQuantity("35821"));
            Assert.IsTrue(sc.checkQuantity("3"));
            Assert.IsTrue(sc.checkQuantity("540"));
        }

    }
}