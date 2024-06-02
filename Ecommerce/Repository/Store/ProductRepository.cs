using Ecommerce.Models.Store;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Configuration;
using System.Linq;
using System.Web;
using System.IO;
using Ecommerce.ViewModel;

namespace Ecommerce.Repository.Store
{
    public class ProductRepository
    {
        private readonly string SQLStr = ConfigurationManager.AppSettings["SQLStr"];

        public List<Distributor> GetDistributors()
        {
            var list = new List<Distributor>();

            using(var conn = new SqlConnection(SQLStr))
            {
                conn.Open();
                using(var cmd = conn.CreateCommand()) {

                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandText = "SELECT * FROM DISTRIBUTOR";
                    using(var reader = cmd.ExecuteReader())
                    {
                        while(reader.Read()) {

                            var dist = new Distributor
                            {
                                D_ID = Convert.ToInt32(reader["D_ID"]),
                                D_NAME = reader["D_ID"].ToString(),
                                D_DATE_ENTERED = reader["D_DATE_ENTERED"].ToString(),
                                D_TIME_ENTERED = reader["D_TIME_ENTERED"].ToString(),
                            };
                            list.Add(dist);
                        }
                    }
                }
                return list;
            }
        }

        public List<object> ProdIdSearch(Product prod)
        {
            var info = new List<object>();
            using (var conn = new SqlConnection(SQLStr))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandText = "SELECT P.PROD_MAKE, P.PROD_MODEL, P.PROD_WARRANTY, P.PROD_DESC, PP.PP_AMOUNT, PQ.PQ_QTY, D.D_ID, D.D_NAME " +
                      "FROM PRODUCT P " +
                      "INNER JOIN PRODUCT_PRICE PP ON P.PROD_ID = PP.PROD_ID " +
                      "INNER JOIN PRODUCT_QUANTITY PQ ON P.PROD_ID = PQ.PROD_ID " +
                      "INNER JOIN DISTRIBUTOR D ON P.D_ID = D.D_ID " +
                      "WHERE P.PROD_ID = @PROD_ID;";

                    cmd.Parameters.AddWithValue("@PROD_ID", prod.PROD_ID);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            info.Add(reader["PROD_MAKE"]);
                            info.Add(reader["PROD_MODEL"]);
                            info.Add(reader["PROD_WARRANTY"]);
                            info.Add(reader["PP_AMOUNT"]);
                            info.Add(reader["PQ_QTY"]);
                            info.Add(reader["PROD_DESC"]);
                            info.Add(reader["D_NAME"]);
                            info.Add(reader["D_ID"]);
                        }
                    }

                }
            }
            if (info.Count > 0)
            {
                return info;
            }
            else
            {
                throw new Exception();
            }
        }

        public bool ProdUpdate(ProductViewData prodView)
        {
            int row = 0;
            using (var conn = new SqlConnection(SQLStr))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandText = "UPDATE PRODUCT SET PROD_MAKE = @PROD_MAKE, PROD_MODEL = @PROD_MODEL, PROD_WARRANTY = @PROD_WARRANTY, PROD_DESC = @PROD_DESC, D_ID = @D_ID WHERE PROD_ID = @PROD_ID; UPDATE PRODUCT_QUANTITY SET PQ_QTY = @PQ_QTY WHERE PROD_ID = @PROD_ID; UPDATE PRODUCT_PRICE SET PP_AMOUNT = @PP_AMOUNT WHERE PROD_ID = @PROD_ID;";
                    cmd.Parameters.AddWithValue("@PROD_ID", prodView.Products.PROD_ID);
                    cmd.Parameters.AddWithValue("@PROD_MAKE", prodView.Products.PROD_MAKE);
                    cmd.Parameters.AddWithValue("@PROD_MODEL", prodView.Products.PROD_MODEL);
                    cmd.Parameters.AddWithValue("@PROD_WARRANTY", prodView.Products.PROD_WARRANTY);
                    cmd.Parameters.AddWithValue("@PROD_DESC", prodView.Products.PROD_DESC);
                    cmd.Parameters.AddWithValue("@D_ID", prodView.Distributor.D_ID); // Update the distributor ID associated with the product
                    cmd.Parameters.AddWithValue("@PQ_QTY", prodView.ProductQty.PQ_QTY);
                    cmd.Parameters.AddWithValue("@PP_AMOUNT", Convert.ToDecimal(prodView.ProductPrices.PP_PRICE));
                    row = cmd.ExecuteNonQuery();

                }
            }

            return row > 0 ? true : throw new Exception();
        }




        public bool ProdDelete(Product prod)
        {
            int row = 0;
            string imageName = null;

            using (var conn = new SqlConnection(SQLStr))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    // First, select the image's file name
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandText = "SELECT PROD_IMG FROM PRODUCT WHERE PROD_ID = @PROD_ID;";
                    cmd.Parameters.AddWithValue("@PROD_ID", prod.PROD_ID);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            imageName = reader["PROD_IMG"].ToString();
                        }
                    }

                    // Then, delete the product from the database
                    cmd.CommandText = "DELETE FROM PRODUCT WHERE PROD_ID = @PROD_ID;";
                    row = cmd.ExecuteNonQuery();
                }
            }

            // If the product was deleted successfully and the image's file name is not null or empty,
            // delete the image from the Images directory
            if (row > 0 && !string.IsNullOrEmpty(imageName))
            {
                string path = ConfigurationManager.AppSettings["imgSavePath"];
                string imagePath = Path.Combine(path, imageName);
                if (File.Exists(imagePath))
                {
                    File.Delete(imagePath);
                }
            }

            return row > 0 ? true : throw new Exception();
        }

        // Method responsible for customer page display
        public List<ProductViewData> FetchAllProduct()
        {
            var prodList = new List<ProductViewData>();
            using (var conn = new SqlConnection(SQLStr))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandText = "SELECT PRODUCT.PROD_ID, PROD_MAKE, PROD_MODEL, PROD_IMG, PP.PP_AMOUNT, PQ.PQ_QTY, PROD_DESC FROM PRODUCT_PRICE AS PP" +
                        " INNER JOIN PRODUCT ON PRODUCT.PROD_ID = PP.PROD_ID" +
                        " INNER JOIN PRODUCT_QUANTITY AS PQ" +
                        " ON PQ.PROD_ID = PRODUCT.PROD_ID;";
                    
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var productViewData = new ProductViewData()
                            {
                                Products = new Product
                                {
                                    PROD_ID = Convert.ToInt32(reader["PROD_ID"]),
                                    PROD_MAKE = reader["PROD_MAKE"].ToString(),
                                    PROD_MODEL = reader["PROD_MODEL"].ToString(),
                                    PROD_IMG = reader["PROD_IMG"].ToString(),
                                    PROD_DESC = reader["PROD_DESC"].ToString()
                                },
                                ProductPrices = new ProductPrice
                                {
                                    PP_PRICE = reader["PP_AMOUNT"].ToString()
                                },
                                ProductQty = new ProductQuantity
                                {
                                    PQ_QTY = (int)reader["PQ_QTY"]
                                }
                             };
                            prodList.Add(productViewData);
                        }
                    }

                }
            }
            if (prodList.Count > 0)
            {
                return prodList;
            }
            else
            {
                throw new Exception();
            }
        }

        // Used for searching
        public List<ProductViewData> FetchProductById(string key)
        {
            var prodList = new List<ProductViewData>();
            using (var conn = new SqlConnection(SQLStr))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandText = "SELECT PROD_MAKE, PROD_MODEL, PROD_IMG, PP.PP_AMOUNT, PQ.PQ_QTY FROM PRODUCT_PRICE AS PP" +
                        " INNER JOIN PRODUCT ON PRODUCT.PROD_ID = PP.PROD_ID" +
                        " INNER JOIN PRODUCT_QUANTITY AS PQ" +
                        " ON PQ.PROD_ID = PRODUCT.PROD_ID" +
                        "WHERE LIKE %PRODUCT.PROD_ID = @PROD_ID;";

                    cmd.Parameters.AddWithValue("@PROD_ID", key);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var productViewData = new ProductViewData()
                            {
                                Products = new Product
                                {
                                    PROD_MAKE = reader["PROD_MAKE"].ToString(),
                                    PROD_MODEL = reader["PROD_MODEL"].ToString(),
                                    PROD_IMG = reader["PROD_IMG"].ToString()
                                },
                                ProductPrices = new ProductPrice
                                {
                                    PP_PRICE = reader["PP_AMOUNT"].ToString()
                                },
                                ProductQty = new ProductQuantity
                                {
                                    PQ_QTY = (int)reader["PQ_QTY"]
                                },
                                IsSearch = true
                            };
                            prodList.Add(productViewData);
                        }
                    }

                }
            }
            if (prodList.Count > 0)
            {
                return prodList;
            }
            else
            {
                throw new Exception();
            }
        }
        public bool CheckProduct(string id)
        {
            string prodId = "";
            using (var conn = new SqlConnection(SQLStr))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandText = "SELECT PROD_ID FROM CART_ITEM WHERE PROD_ID = @PROD_ID;";
                    cmd.Parameters.AddWithValue("@PROD_ID", id);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            prodId = reader["PROD_ID"].ToString();
                        }
                    }
                }
            }
            return string.IsNullOrEmpty(prodId) ? false : true;
        }
    }
}