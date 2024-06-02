using Ecommerce.Models;
using Ecommerce.Models.User;
using Ecommerce.Models.Store;
using Ecommerce.Repository.Store;
using Ecommerce.Repository.User;
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
using System.Web.Routing;
using System.Web.Services.Description;
using System.Xml.Linq;
using WebGrease.Css.Ast.Selectors;
using Antlr.Runtime.Tree;
using System.Security.Policy;
using System.Globalization;


namespace Ecommerce.Controllers
{
    public class StoreController : Controller
    {

        private List<UserViewData> SearchUserId(string id)
        {
            int userId = Convert.ToInt32(id);

            try
            {
                UserRepository user = new UserRepository();
                var result = user.GetIndividualUser(userId);

                return result;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Exception searching user id: " + e);
                return new List<UserViewData>();
            }
        }

        private bool UserUpdateInfo(string[] array)
        {
            int prodId = Convert.ToInt32(array[0]);
            try
            {
                UserViewData userViewData = new UserViewData
                {
                    UserModel = new UserModel
                    {
                        USER_ID = prodId,
                        USER_FNAME = array[1],
                        USER_LNAME = array[2],
                        USER_PHONE = array[3],
                    },
                    Address = new Address
                    {
                        AD_STREET = array[5],
                        AD_BRGY = array[6],
                        AD_CITY = array[7],
                        AD_PROVINCE = array[8],
                        AD_ZIPCODE = array[9],
                    },
                    Credential = new Credential
                    {
                        C_EMAIL = array[4],
                    }
                };

                UserRepository user = new UserRepository();
                var result = user.UpdateUser(userViewData);

                return result;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Exception updating a user: " + e);
                return false;
            }
        }

        private bool UserDeleteInfo(string id, string phone)
        {   
            try
            {
                UserRepository user = new UserRepository();
                bool result = user.DeleteUser(id, phone);

                return result;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Exception deleting a user: " + e);
                return false;
            }
        }

        private bool validateUserId(string a)
        {
            return string.IsNullOrEmpty(a) ? true : false;
        }

        public ActionResult UserAdminAction(FormCollection data)
        {
            string id = data["id"];
            string clickAction = data["clickAction"];
            string fname = data["fname"];
            string lname = data["lname"];
            string phone = data["phone"];
            string email = data["email"];
            string street = data["street"];
            string brgy = data["brgy"];
            string city = data["city"];
            string province = data["province"];
            string zip = data["zip"];


            string[] dataConllection = { id, fname, lname, phone, email, street, brgy, city, province, zip };

            bool idValidation = validateUserId(id);

            bool validateFields = InputValidation(dataConllection);

            if(idValidation == true && clickAction == "Search")
            {
                return Json(new
                {
                    response = false,
                    content = "Id is empty."
                });
            }

            if(clickAction != "Search" && !string.IsNullOrEmpty(email))
            {
                if (validateFields)
                {
                    return Json(new
                    {
                        response = false,
                        content = "Please check for empty fields."
                    });
                }
            }

            switch (clickAction)
            {
                case "Search":
                    var contentCount = SearchUserId(id).Count();
                    var content = SearchUserId(id);
                    bool res = false;
                    if (contentCount < 1)
                    {
                        return Json(new
                        {
                            response = res,
                            content = "No user found."
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
                    res = UserUpdateInfo(dataConllection);
                    return Json(new
                    {
                        action = clickAction,
                        response = res
                    });
                case "Delete":
                    res = UserDeleteInfo(id, phone);
                    if(!res)
                    {
                        return Json(new
                        {
                            action = clickAction,
                            response = res,
                            content = "Input mismatch or user not found."
                        });
                    }
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

        public ActionResult CustomerPage()
        {
            if ((string)Session["Role"] != "Admin")
            {
                return RedirectToAction("Index", "LogIn");
            }

            try
            {
                UserRepository userRepo = new UserRepository();
                var display = userRepo.GetUsers();
                return View(display);
            } catch(Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Exception caught: " + e);
                return View();
            }
        }

        private bool InputValidation(string[] arr)
        {
            bool isNull = false;
            for(int i = 0; i < arr.Length; i++)
            {
                if (string.IsNullOrEmpty(arr[i]))
                {
                    isNull = true;
                    break;
                }
            }
            return isNull;
        }

        private bool AdminAuth(string[] fields)
        {

            Credential admin = new Credential
            {
                C_EMAIL = fields[0],
                C_PASS = fields[1]
            };

            AdminRepository auth = new AdminRepository();
            object[] adminData = auth.AdminAuth(admin);

            try
            {
                if (!(bool)adminData[0])
                {
                    return false;
                }
            } catch(Exception e)
            {
                Debug.WriteLine("Exception in inserting data: " + e);
            }



            Session["adminEmail"] = adminData[1];
            Session["adminPass"] = adminData[2];
            Session["adminId"] = adminData[3];
            Session["adminFname"] = adminData[4];
            Session["adminLname"] = adminData[5];
            Session["Role"] = "Admin";

            return true;
        }

        public ActionResult AdminAuthentication(FormCollection form)
        {
            string[] fields = { form["cus_email"], form["cus_pass"]};
            bool validate = InputValidation(fields);

            if (validate == true)
            {
                return Json(new
                {
                    response = false,
                    mess = "Please fill up the missing fields."
                });
            }

            bool auth = AdminAuth(fields);

            if (!auth)
            {
                return Json(new
                {
                    response = false,
                    mess = "Invalid credentials. Please contact administrator."
                });
            }


            // Get the URL of the Admin page
            string redirectUrl = Url.Action("Admin", "Store");

            return Json(new
            {
                response = true,
                mess = "Log in successful.",
                redirectUrl = redirectUrl  // Add the redirect URL to the response
            });
        }
        
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
            if ((string)Session["Role"] != "Admin")
            {
                return RedirectToAction("Index", "LogIn");
            }
            

            try
            {
                var products = await LoadProducts();

                if (products == null)
                {
                    return View();
                }
                return View(products);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Error occured while fetching products: " + e);
                return View();
            }
        }

        private List<object> SearchProId(string id)
        {
            try
            {
                int prodId = Convert.ToInt32(id);
                Product prod = new Product
                {
                    PROD_ID = prodId
                };

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
            try
            {
                int prodId = Convert.ToInt32(array[0]);
                ProductViewData prodView = new ProductViewData
                {
                    Products = new Product
                    {
                        PROD_ID = prodId,
                        PROD_MAKE = array[1],
                        PROD_MODEL = array[2],
                        PROD_WARRANTY = Convert.ToInt32(array[3]),
                        PROD_DESC = array[6],
                    },
                    ProductQty = new ProductQuantity
                    {
                        PQ_QTY = Convert.ToInt32(array[4])
                    },
                    ProductPrices = new ProductPrice
                    {
                        PP_PRICE = array[5],
                    },
                    Distributor = new Distributor
                    {
                        D_ID = Convert.ToInt32(array[7]),
                        D_NAME = array[8],
                    }
                };
                ProductRepository product = new ProductRepository();
                bool result = product.ProdUpdate(prodView);

                return result;
            }catch(Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Exception updating the product: " + e);
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
                System.Diagnostics.Debug.WriteLine("Exception deleting the product: " + e);
                return false;
            }
        }


        [HttpPost]
        public ActionResult ProductUpdate(FormCollection data)
        {
            string prodId = data["id"];
            string clickAction = data["clickAction"];
            string make = data["make"];
            string model = data["model"];
            string warranty = data["warranty"];
            string qty = data["quantity"];

            string desc = data["desc"];
            string distId = data["distributor"];
            string distName = data["distName"];

            string price = data["price"];

            string[] dataConllection = { prodId, make, model, warranty, qty, price, desc, distId, distName };

            bool validateInputs = InputValidation(dataConllection);

            if (clickAction != "Search")
            {
                if (validateInputs)
                {
                    return Json(new
                    {
                        response = false,
                        content = "Make sure there are no missing fields."
                    });
                }
            }

            switch (clickAction)
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
                        response = res,
                        content = "Unable to update, no product found."
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
            // Check session roles
            if ((string)Session["Role"] != "Admin")
            {
                return RedirectToAction("Index", "LogIn");
            }
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
            if ((string)Session["Role"] != "Admin")
            {
                return RedirectToAction("Index", "LogIn");
            }
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
        public ActionResult RegisterAdmin(FormCollection data)
        {

            string firstName = data["firstname"];
            string lastName = data["lastname"];
            string middleName = data["middlename"];
            string email = data["email"];
            string phone = data["phone"];
            string street = data["street"];
            string barangay = data["baranggay"];
            string city = data["city"];
            string province = data["province"];
            string zipCode = data["zipcode"];
            string password = data["password"];

            string[] formData =
             {
                firstName,
                lastName,
                email,
                phone,
                street,
                barangay,
                city,
                province,
                zipCode,
                password
             };

            if(zipCode.Length != 4)
            {
                return Json(new
                {
                    response = false,
                    content = "Zip code must be exactly 4 digits."
                });
            }

            if (phone.Length != 11)
            {
                return Json(new
                {
                    response = false,
                    content = "Phone must be exactly 11 digits."
                });
            }

            bool isValidated = InputValidation(formData);

            if(isValidated)
            {
                return Json(new
                {
                    response = false,
                    content = "Inputs must not be null."
                });
            }

            AdminRepository adminRepo = new AdminRepository();
            Admin admin = new Admin();

            if(string.IsNullOrEmpty(middleName))
            {
                admin = new Admin
                {
                    A_FNAME = NormalizeStr(firstName),
                    A_LNAME = NormalizeStr(lastName),
                    A_MNAME = middleName,
                    A_PHONE = phone
                };
            } 
            else
            {
                admin = new Admin
                {
                    A_FNAME = NormalizeStr(firstName),
                    A_LNAME = NormalizeStr(lastName),
                    A_MNAME = NormalizeStr(middleName),
                    A_PHONE = phone
                };
            }

            int adminId = 0;

            try
            {
                adminId = adminRepo.AdminDetails(admin);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception catched: " + e);
            }

            Address address = new Address
            {
                AD_STREET = NormalizeStr(street),
                AD_BRGY = NormalizeStr(barangay),
                AD_PROVINCE = NormalizeStr(province),
                AD_CITY = NormalizeStr(city),
                AD_ZIPCODE = NormalizeStr(zipCode),
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
                Debug.WriteLine("Exception catched: " + e);
            }

            Credential credential = new Credential
            {
                C_EMAIL = email,
                C_PASS = password,
                A_ID = adminId.ToString()
            };

            int credId = 0;
            try
            {
                credId = adminRepo.AdminCredential(credential);

                if( credId == 0 )
                {
                    return Json(new
                    {
                        response = false,
                        content = "Something went wrong."
                    });
                }

                return Json(new
                {
                    response = true,
                    content = "User registered!"
                });
            }
            catch (Exception e)
            {

                Debug.WriteLine("Exception catched: " + e);
                return Json(new
                {
                    response = false,
                    content = "Inputs must not be null."
                });
            }
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
                Debug.WriteLine("Exception caught while looking of admin I.D. : " + e);
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

        public bool ValidateAcquisitionDate(string acquiredDate)
        {
            // Parse the input date string
            DateTime parsedDate;
            if (!DateTime.TryParseExact(acquiredDate, "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate))
            {
                return false;
            }

            // Get the current date (without time part)
            DateTime currentDate = DateTime.Today;

            // Calculate the date 5 months ago from today
            DateTime fiveMonthsAgo = currentDate.AddMonths(-5);

            // Check if the parsed date is before today and within the past 5 months
            if (parsedDate > currentDate || parsedDate < fiveMonthsAgo)
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

            string userId = data["userId"];
            string make = data["make"];
            string model = data["model"];
            string distributor = data["distributor"];
            string purchaseDate = data["purchase-date"];
            string price = data["price"];
            string quantity = data["quantity"];
            string warranty = data["warranty"];
            string desc = data["desc"];

            string[] formData =
            {
                userId,
                make,
                model,
                distributor,
                purchaseDate,
                price,
                quantity,
                warranty,
                desc,
            };


            bool validateInputs = InputValidation(formData);

            if(validateInputs)
            {
                return Json(new
                {
                    success = false,
                    response = "Fields can not be empty."
                });
            }

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
                A_ID = userId
            };

            bool isFound = SearchAdmin(adminId);

            if(!isFound)
            {
                return Json(new
                {
                    success = false, 
                    response = "Please enter a valid admin I.D."
                });
            }

           
            bool validateDate = ValidateWarranty(warranty);

            if (!validateDate)
            {
                return Json(new
                {
                    success = false,
                    response = "Date should be atleast a month after."
                });
            }

            string initialPrice = price;
            bool checkPrice = CheckPriceEntry(initialPrice);
            if(!checkPrice)
            {
                return Json(new
                {
                    success = false,
                    response = "Price should not be lesser than 0."
                });
            }

            string initialQty = quantity;
            bool checkQty = checkQuantity(initialQty);
            if(!checkQty)
            {
                return Json(new
                {
                    success = false,
                    response = "Quantity should not be lesser than 1."
                });
            }

            //bool productValidator = ValidateAcquisitionDate(purchaseDate);
            //if (!productValidator)
            //{
            //    return Json(new
            //    {
            //        success = false,
            //        response = "Purchase date should be atleast a day before or at current date."
            //    });
            //}

            int prodWarranty = Convert.ToInt32(NormalizeStr(warranty));

            Product product = new Product
            {
                PROD_MAKE = make,
                PROD_MODEL = model,
                PROD_WARRANTY = prodWarranty,
                PROD_DESC = NormalizeStr(desc),
                PROD_IMG = prodImg,
                A_ID = Convert.ToInt32(userId),
                D_ID = Convert.ToInt32(distributor)
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
                PA_DATE = purchaseDate,
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

            ProductPrice productPrice = new ProductPrice
            {
                PP_PRICE = (PriceValidation(data["price"])),
                PROD_ID = prodId
            };

            bool priceValidation = PriceInsert(productPrice);
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
                PQ_QTY = Convert.ToInt32(quantity),
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