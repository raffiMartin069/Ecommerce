using Ecommerce.Models.Store;
using Ecommerce.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Reflection;
using System.Threading.Tasks;

namespace Ecommerce.Repository.Store
{
    public class ProductEntryRepository
    {
        private string SQLString()
        {
            return ConfigurationManager.AppSettings["SQLStr"];
        }

        // Asynchronous function is used to fully execute while page is loading.
        // Data will be rendered without blocking or stoping other operations.
        // Useful for large data retrievale from database.
        // In this case, this is somewhat of a practice application of Task/Asynchronous.
        public async Task<List<Distributor>> GetAllDistributor()
        {
            var distributors = new List<Distributor>();
            try
            {
                using (var conn = new SqlConnection(SQLString()))
                {
                    conn.Open();
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandType = System.Data.CommandType.Text;
                        cmd.CommandText = "SELECT * FROM DISTRIBUTOR;";
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (await reader.ReadAsync())
                            {
                                Distributor dist = new Distributor
                                {
                                    D_NAME = reader["D_NAME"].ToString(),
                                    D_ID = (int) reader["D_ID"]
                                };
                                distributors.Add(dist);
                            }
                        }
                    }
                }
                return distributors;
            }
            catch (Exception e)
            {
                throw new Exception("Exception thrown" + e);
            }
        }

        public async Task<List<ProductViewData>> GetAllProducts()
        {
            var prodList = new List<ProductViewData>();
            try
            {
                using (var conn = new SqlConnection(SQLString())) // Added closing parenthesis here
                {
                    conn.Open();
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandType = System.Data.CommandType.Text;
                        cmd.CommandText = "SELECT P.PROD_ID," +
                            " P.PROD_MAKE," +
                            " P.PROD_MODEL," +
                            " P.PROD_WARRANTY," +
                            " P.PROD_DESC," +
                            " PQ.PQ_QTY," +
                            " PP_AMOUNT" +
                            " FROM PRODUCT AS P" +
                            " INNER JOIN PRODUCT_QUANTITY AS PQ" +
                            " ON P.PROD_ID = PQ.PROD_ID" +
                            " INNER JOIN PRODUCT_PRICE AS PP" +
                            " ON P.PROD_ID = PP.PROD_ID;";

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (await reader.ReadAsync())
                            {
                                ProductViewData prodView = new ProductViewData
                                {
                                    Products = new Product
                                    {
                                        PROD_ID = Convert.ToInt32(reader["PROD_ID"]),
                                        PROD_MAKE = reader["PROD_MAKE"].ToString(),
                                        PROD_MODEL = reader["PROD_MODEL"].ToString(),
                                        PROD_WARRANTY = Convert.ToInt32(reader["PROD_WARRANTY"]),
                                        PROD_DESC = reader["PROD_DESC"].ToString(),
                                    },
                                    ProductQty = new ProductQuantity
                                    {
                                        PQ_QTY = Convert.ToInt32(reader["PQ_QTY"])
                                    },
                                    ProductPrices = new ProductPrice
                                    {
                                        PP_PRICE = reader["PP_AMOUNT"].ToString()
                                    }
                                    
                            };

                                prodList.Add(prodView);
                            }
                        }
                    }
                }
                return prodList;
            }
            catch (Exception e)
            {
                throw new Exception("Exception thrown: " + e);
            }
        }

        public bool SearchAdmin(Admin id)
        {
            bool isFound = true;
            try
            {
                using (var conn = new SqlConnection(SQLString()))
                {
                    conn.Open();
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandType = System.Data.CommandType.Text;
                        cmd.CommandText = "SELECT A_ID FROM ADMIN WHERE A_ID = @ID";
                        cmd.Parameters.AddWithValue("@ID", id.A_ID);
                        using(SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if(!reader.Read())
                            {
                                isFound = false;
                            }
                        }
                    }
                }
                return isFound;
            }
            catch (Exception)
            {
                throw new Exception("The I.D. does not exist.");
            }
        }

        public int ProductInsert(Product prod)
        {
            try
            {
                using (var conn = new SqlConnection(SQLString()))
                {
                    conn.Open();
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandType = System.Data.CommandType.Text;
                        cmd.CommandText = "INSERT INTO PRODUCT(PROD_MAKE, PROD_MODEL, PROD_WARRANTY, PROD_DESC, PROD_IMG, A_ID, D_ID) " +
                            "VALUES(@PROD_MAKE, @PROD_MODEL, @PROD_WARRANTY, @PROD_DESC, @PROD_IMG, @A_ID, @D_ID); SELECT SCOPE_IDENTITY();";

                        cmd.Parameters.AddWithValue("@PROD_MAKE", prod.PROD_MAKE);
                        cmd.Parameters.AddWithValue("@PROD_MODEL", prod.PROD_MODEL);
                        cmd.Parameters.AddWithValue("@PROD_WARRANTY", prod.PROD_WARRANTY);
                        cmd.Parameters.AddWithValue("@PROD_DESC", prod.PROD_DESC);
                        cmd.Parameters.AddWithValue("@PROD_IMG", prod.PROD_IMG);
                        cmd.Parameters.AddWithValue("@A_ID", prod.A_ID);
                        cmd.Parameters.AddWithValue("@D_ID", prod.D_ID);
                        
                        
                        // return id for reference to other tables.
                        int id = Convert.ToInt32(cmd.ExecuteScalar());
                        return id;
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public bool PriceInsert(ProductPrice price)
        {
            bool isInserted = true;
            try
            {
                using (var conn = new SqlConnection(SQLString()))
                {
                    conn.Open();
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandType = System.Data.CommandType.Text;
                        cmd.CommandText = "INSERT INTO PRODUCT_PRICE(PP_AMOUNT, PROD_ID) VALUES(@PP_AMOUNT, @PROD_ID)";
                        cmd.Parameters.AddWithValue("@PP_AMOUNT", Convert.ToDouble(price.PP_PRICE));
                        cmd.Parameters.AddWithValue("@PROD_ID", Convert.ToDecimal(price.PROD_ID));
                        int row = cmd.ExecuteNonQuery();
                        if(row < 1)
                        {
                            isInserted = false;
                        }
                    }
                }
                return isInserted;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public bool QuantityInsert(ProductQuantity qty)
        {
            bool isInserted = true;
            try
            {
                using (var conn = new SqlConnection(SQLString()))
                {
                    conn.Open();
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandType = System.Data.CommandType.Text;
                        cmd.CommandText = "INSERT INTO PRODUCT_QUANTITY(PQ_QTY, PROD_ID) VALUES(@PQ_QTY, @PROD_ID)";
                        cmd.Parameters.AddWithValue("@PQ_QTY", qty.PQ_QTY);
                        cmd.Parameters.AddWithValue("@PROD_ID", qty.PROD_ID);
                        int row = cmd.ExecuteNonQuery();
                        if (row < 1)
                        {
                            isInserted = false;
                        }
                    }
                }
                return isInserted;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public bool ProductAcquired(ProductAcquisition pa)
        {
            try
            {
                using (var conn = new SqlConnection(SQLString()))
                {
                    conn.Open();
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandType = System.Data.CommandType.Text;
                        cmd.CommandText = "INSERT INTO PRODUCT_ACQUISITION(PA_DATE, PROD_ID) VALUES(@PA_DATE, @PROD_ID)";
                        cmd.Parameters.AddWithValue("@PA_DATE", pa.PA_DATE);
                        cmd.Parameters.AddWithValue("@PROD_ID", pa.PROD_ID);

                        if(pa.PA_DATE != null && pa.PA_ID != 0)
                        {
                            cmd.CommandText = "INSERT INTO PRODUCT_LOG(PL_DATE, PL_TIME, PROD_ID) VALUES(DEFAULT, DEFAULT, @PROD_ID)";
                            cmd.Parameters.AddWithValue("@PROD_ID", pa.PROD_ID);
                        }
                        int row = cmd.ExecuteNonQuery();
                        
                        if (row < 1)
                        {
                            return false;
                        }
                        ProductLog(pa.PROD_ID);
                        
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void ProductLog(int prodId)
        {
            try
            {
                using (var conn = new SqlConnection(SQLString()))
                {
                    conn.Open();
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "INSERT INTO PRODUCT_LOG(PL_DATE, PL_TIME, PROD_ID) VALUES(DEFAULT, DEFAULT, @PROD_ID)";
                        cmd.Parameters.AddWithValue("@PROD_ID", prodId);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}