using Ecommerce.Models;
using Ecommerce.Models.Store;
using Ecommerce.Repository.Store;
using Ecommerce.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Services.Description;
using System.Xml.Linq;
using WebGrease.Css.Ast.Selectors;


namespace Ecommerce.Controllers
{
    public class StoreController : Controller
    {
        // GET: Store
        public ActionResult Index()
        {
            return View();
        }

        private async Task<List<ProductViewData>> LoadProducts()
        {
            ProductEntryRepository prod = new ProductEntryRepository();
            return await prod.GetAllProducts();
        }

        public async Task<ActionResult> Admin()
        {
            try
            {
                var products = await LoadProducts();
                if (products == null)
                {
                    return RedirectToAction("Admin");
                }
                return View(products);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Error occured while fetching products: " + e);
                return RedirectToAction("Admin");
            }
        }

        private List<object> SearchProId(string id)
        {
            int prodId = Convert.ToInt32(id);
            
            Product prod = new Product
            {
                PROD_ID = prodId
            };

            try
            {
                ProductRepository product = new ProductRepository();
                var result = product.ProdIdSearch(prod);

                return result;
            } catch(Exception e) {
                System.Diagnostics.Debug.WriteLine("Exception searching product id: " + e);
                return new List<object>();
            }
        }

        private bool ProductUpdateInfo(string[] array)
        {
            int prodId = Convert.ToInt32(array[0]);
            try
            {
                ProductViewData prodView = new ProductViewData
                {
                    Products = new Product
                    {
                        PROD_ID = prodId,
                        PROD_MAKE = array[1],
                        PROD_MODEL = array[2],
                        PROD_WARRANTY = Convert.ToInt32(array[3])
                    },
                    ProductQty = new ProductQuantity
                    {
                        PQ_QTY = Convert.ToInt32(array[4])
                    },
                    ProductPrices = new ProductPrice
                    {
                        PP_PRICE =array[5],
                    }
                };
                ProductRepository product = new ProductRepository();
                bool result = product.ProdUpdate(prodView);

                return result;
            }catch(Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Exception searching updating the product: " + e);
                return false;
            }
        }

        private bool ProductDeleteInfo(string[] array)
        {
            int prodId = Convert.ToInt32(array[0]);
            Product prod = new Product
            {
                PROD_ID = prodId
            };
            try
            {
                ProductRepository product = new ProductRepository();
                bool result = product.ProdDelete(prod);

                return result;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Exception searching updating the product: " + e);
                return false;
            }
        }


        [HttpPost]
        public ActionResult ProductUpdate(FormCollection data)
        {
            string prodId = data["id"];
            string clickAction = data["action"];
            string make = data["make"];
            string model = data["model"];
            string warranty = data["warranty"];
            string qty = data["quantity"];

            string price = data["price"];
            

            string[] dataConllection = { prodId, make, model, warranty, qty, price };

            switch(clickAction)
            {
                case "Search":
                    var contentCount = SearchProId(prodId).Count();
                    var content = SearchProId(prodId);
                    bool res = false;
                    if (contentCount < 1)
                    {
                        return Json(new
                        {
                            response = res,
                            content = "No product found."
                        });
                    }
                    else
                    {
                        res = true;
                        return Json(new
                        {
                            action = clickAction,
                            response = res,
                            contents = content
                        });
                    }
                    
                case "Update":
                    res = ProductUpdateInfo(dataConllection);
                    return Json(new
                    {
                        action = clickAction,
                        response = res
                    });
                case "Delete":
                    res = ProductDeleteInfo(dataConllection);
                    return Json(new
                    {
                        action = clickAction,
                        response = res
                    });
                  
                default:
                    break;
            }

            return Json(new
            {
                success = true,
                response = clickAction
            });
        }

        private async Task<List<Distributor>> LoadDistributors()
        {
            ProductEntryRepository rep = new ProductEntryRepository();
            return await rep.GetAllDistributor();
        }

        public async Task<ActionResult> ProductEntry()
        {
            try
            {
                var distributors = await LoadDistributors();
                if (distributors == null)
                {
                    return RedirectToAction("ProductEntry");
                }

                return View(distributors);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Error occured while doing an operation: " + e);
                return RedirectToAction("ProductEntry");
            }
        }

        public ActionResult Register()
        {
            return View();
        }

        private bool AdminRegistration(int adminId, int addressId, int credId)
        {
            bool flag = true;
            if (adminId == 0 && addressId == 0 && credId == 0)
            {
                flag = false;
            }
            return flag;
        }

        [HttpPost]
        public ActionResult Register(FormCollection data)
        {
            AdminRepository adminRepo = new AdminRepository();
            Admin admin = new Admin
            {
                A_FNAME = NormalizeStr(data["firstname"]),
                A_LNAME = NormalizeStr(data["lastname"]),
                A_MNAME = NormalizeStr(data["middlename"]),
                A_PHONE = data["phone"]
            };

            int adminId = 0;

            try
            {
                adminId = adminRepo.AdminDetails(admin);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Exception catched: " + e);
            }

            Address address = new Address
            {
                AD_STREET = NormalizeStr(data["street"]),
                AD_BRGY = NormalizeStr(data["baranggay"]),
                AD_PROVINCE = NormalizeStr(data["province"]),
                AD_CITY = NormalizeStr(data["city"]),
                AD_ZIPCODE = NormalizeStr(data["zipcode"]),
                A_ID = adminId.ToString()
            };

            // for future references
            int addressId = 0;
            try
            {
                addressId = adminRepo.AdminAddress(address);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Exception catched: " + e);
            }

            Credential credential = new Credential
            {
                C_EMAIL = data["email"],
                C_PASS = data["password"],
                A_ID = adminId.ToString()
            };

            int credId = 0;
            try
            {
                credId = adminRepo.AdminCredential(credential);
            }
            catch (Exception e)
            {

                System.Diagnostics.Debug.WriteLine("Exception catched: " + e);
            }

            bool registrationResult = AdminRegistration(adminId, addressId, credId);

            return Json(registrationResult);
        }

        private bool SearchAdmin(Admin id)
        {
            ProductEntryRepository idSearch = new ProductEntryRepository();
            try
            {
                bool found = idSearch.SearchAdmin(id);
                return found;
            } catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Exception caught while looking of admin I.D. : " + e);
                return false;
            }
        }

        private int InsertProduct(Product prod)
        {
            ProductEntryRepository product = new ProductEntryRepository();
            int id = 0;
            try
            {
                id = product.ProductInsert(prod);
                return id;
            }
            catch(Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Exception caught while inserting product: " + e);
            }
            return id;
        }

        private bool PriceInsert(ProductPrice price)
        {
            ProductEntryRepository product = new ProductEntryRepository();
            try
            {
                bool isInserted = product.PriceInsert(price);
                return isInserted;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Exception caught while inserting product price: " + e);
                return false;
            }
        }

        private bool QuantityInsert(ProductQuantity qty)
        {
            ProductEntryRepository product = new ProductEntryRepository();
            try
            {
                bool isInserted = product.QuantityInsert(qty);
                return isInserted;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Exception caught while inserting product quantity: " + e);
                return false;
            }
        }

        // This simply tells us "When was the product bought".
        private bool ProductDateAcquired(ProductAcquisition pa)
        {
            ProductEntryRepository product = new ProductEntryRepository();
            try
            {
                bool isInserted = product.ProductAcquired(pa);
                return isInserted;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Exception caught while inserting product date acquisition: " + e);
                return false;
            }
        }

        private bool SaveProductImage(string img, HttpPostedFileBase formImg)
        {
            string path = ConfigurationManager.AppSettings["imgSavePath"];
            string combine = Path.Combine(path, img);

            if(path == null || combine == null)
            {
                return false;
            }

            formImg.SaveAs(combine);
            return true;
        }


        public object[] ImageValidation(HttpPostedFileBase prodimg)
        {
            string[] allowedExtension = { ".jpg", ".jpeg", ".svg", ".png"};
            string img = Path.GetExtension(prodimg.FileName);
            object[] response = new object[4];
            
            if (prodimg == null)
            {
                response[0] = false;
                response[1] = "Please add an image";
                return response;
            }

            bool isMatched = false;

            foreach(string extension in allowedExtension)
            {
                if (Path.GetExtension(img) == extension)
                {
                    isMatched = true;
                    break;
                }
            }

            if(isMatched == false)
            {
                response[0] = false;
                response[1] = "Only .jpg, .jpeg, .svg and .png are allowed.";
                return response;
            }

            if (prodimg.ContentLength > 20 * 1024 * 1024)
            {
                response[0] = false;
                response[1] = "File size should not be greater than.";
                return response;
            }

            response[0] = true;
            return response;
        }

        private string NormalizeStr(string a)
        {
            a = a.ToLower();
            return char.ToUpper(a[0]) + a.Substring(1);
        }

        private string PriceValidation(string amount)
        {
            decimal money;

            // Attempt to parse the amount as a decimal
            if (decimal.TryParse(amount, out money))
            {
                // Divide by 100 to convert cents to dollars (if needed)
                money = money / 100m;

                // Return the formatted string
                return money.ToString("N2");
            }
            else
            {
                // Handle the case where the input is not a valid number
                // For example, you could return "0.00" or an error message
                return "Invalid amount";
            }
        }

        public bool ValidateWarranty(string warranty)
        {
            int coverage = Convert.ToInt32(warranty);
            return coverage > -1 ? true : false;
        }

        public bool ValidateAcquisitionDate(string aquiredDate)
        {
            string[] date_list = aquiredDate.Split('-');
            // YYY-MMM-DD Formart
            string year = date_list[0];
            string month = date_list[1];
            string date = date_list[2];

            DateTime currentDate = DateTime.Now;

            if (Convert.ToInt32(year) > Convert.ToInt32(currentDate.Year))
            {
                return false;
            }

            if (Convert.ToInt32(month) > Convert.ToInt32(currentDate.Month))
            {
                return false;
            }

            if (Convert.ToInt32(date) > Convert.ToInt32(currentDate.Day))
            {
                return false;
            }

            return true;
        }

        public bool CheckPriceEntry(string price)
        {
            int priceCheck = Convert.ToInt32(price);
            return priceCheck > -1 ? true : false;
        }

        public bool checkQuantity(string quantity)
        {
            int quantityCheck = Convert.ToInt32(quantity);
            return quantityCheck > 0 ? true : false;
        }

        // Passed, data can already be fetched to this endpoint.
        [HttpPost]
        public ActionResult Entries(FormCollection data, HttpPostedFileBase prodimg)
        {
            var imgValidationResponse = ImageValidation(prodimg);
            // THIS WILL BE REVIEWED FIRST
            if (!(bool)imgValidationResponse[0])
            {
                return Json(new
                {
                    success = false,
                    response = imgValidationResponse[1]
                });
            }

            string prodImg = Path.GetFileName(prodimg.FileName);

            // Search through the database for matches, if found we proceed to the process below.
            Admin adminId = new Admin
            {
                A_ID = data["userId"]
            };

            bool isFound = SearchAdmin(adminId);

            if(!isFound)
            {
                return Json(new
                {
                    success = false, response = "Please enter a valid admin I.D."
                });
            }

            string warranty = data["warranty"];
            bool validateDate = ValidateWarranty(warranty);

            if (!validateDate)
            {
                return Json(new
                {
                    success = false,
                    response = "Date should be atleast a month after."
                });
            }

            string initialPrice = data["price"];
            bool checkPrice = CheckPriceEntry(initialPrice);
            if(!checkPrice)
            {
                return Json(new
                {
                    success = false,
                    response = "Price should not be lesser than 0."
                });
            }

            string initialQty = data["quantity"];
            bool checkQty = checkQuantity(initialQty);
            if(!checkQty)
            {
                return Json(new
                {
                    success = false,
                    response = "Quantity should not be lesser than 1."
                });
            }

            bool productValidator = ValidateAcquisitionDate(data["purchase-date"]);
            if (!productValidator)
            {
                return Json(new
                {
                    success = false,
                    response = "Purchase date should be atleast a day before or at current date."
                });
            }

            int prodWarranty = Convert.ToInt32(NormalizeStr(data["warranty"]));

            Product product = new Product
            {
                PROD_MAKE = NormalizeStr(data["make"]),
                PROD_MODEL = NormalizeStr(data["model"]),
                PROD_WARRANTY = prodWarranty,
                PROD_DESC = NormalizeStr(data["desc"]),
                PROD_IMG = prodImg,
                A_ID = Convert.ToInt32(data["userId"]),
                D_ID = Convert.ToInt32(data["distributor"])
            };

            int prodId = InsertProduct(product);
            if(prodId < 1)
            {
                return Json(new
                {
                    success = false,
                    response = "Something went wrong adding the products"
                });
            }

            bool isSaved = SaveProductImage(prodImg, prodimg);
            if (!isSaved)
            {
                return Json(new
                {
                    success = false,
                    response = "Something went wrong in saving the image. We are currently working with it."
                });
            }

            ProductAcquisition prodAcquire = new ProductAcquisition
            {
                PA_DATE = data["purchase-date"],
                PROD_ID = prodId
            };

            bool acquisitionDateValidation = ProductDateAcquired(prodAcquire);
            if (!acquisitionDateValidation)
            {
                return Json(new
                {
                    success = false,
                    response = "Something went wrong adding the date bought. We are currently working with it."
                });
            }

            ProductPrice price = new ProductPrice
            {
                PP_PRICE = (PriceValidation(data["price"])),
                PROD_ID = prodId
            };

            bool priceValidation = PriceInsert(price);
            if (!priceValidation)
            {
                return Json(new
                {
                    success = false,
                    response = "Something went wrong adding the price. We are currently working with it."
                });
            }

            ProductQuantity pq = new ProductQuantity
            {
                PQ_QTY = Convert.ToInt32(data["quantity"]),
                PROD_ID = prodId
            };

            bool qtyValidation = QuantityInsert(pq);
           if (!qtyValidation)
            {
                return Json(new
                {
                    success = false,
                    response = "Something went wrong adding the quantity. We are currently working with it."
                });
            }

            data.Clear();

            return Json(new
            {
                success = true,
                response = "Product has been saved successfuly!",
                title = "Product registered!"
            }); ;
        }
    }
}