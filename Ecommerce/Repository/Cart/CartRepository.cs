using Ecommerce.Models.Cart;
using Ecommerce.Models.Store;
using Ecommerce.Repository.Store;
using Ecommerce.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;

namespace Ecommerce.Repository.Cart
{
    public class CartRepository
    {
        private readonly string SQLStr = ConfigurationManager.AppSettings["SQLStr"];

        public int SearchProductQuantity(int prodId)
        {
            int quantity = 0;
            using(var conn =  new SqlConnection(SQLStr))
            {
                conn.Open();
                using(var cmd = conn.CreateCommand())
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandText = "SELECT PQ_QTY FROM PRODUCT_QUANTITY WHERE PROD_ID = @PROD_ID;";
                    cmd.Parameters.AddWithValue("@PROD_ID", prodId);
                    using(var reader = cmd.ExecuteReader())
                    {
                        if(reader.Read())
                        {
                            quantity = Convert.ToInt32(reader["PQ_QTY"]);
                        }
                    }
                }
            }
            return quantity;
        }

        public bool UpdateCartItemQuantity(int cartId, int quantity)
        {
            int row = 0;
            using (var conn = new SqlConnection(SQLStr))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    // Then, delete the product from the database
                    cmd.CommandText = "UPDATE CART_ITEM SET CI_QTY = @CI_QTY WHERE CI_ID = @CI_ID;";
                    cmd.Parameters.AddWithValue("@CI_QTY", quantity);
                    cmd.Parameters.AddWithValue("@CI_ID", cartId);
                    row = cmd.ExecuteNonQuery();
                }
            }
            return row > 0 ? true : false;
        }

        public bool DeleteCartItem(int cartId)
        {
            int row = 0;
            using (var conn = new SqlConnection(SQLStr))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    // Then, delete the product from the database
                    cmd.CommandText = "DELETE FROM CART_ITEM WHERE CI_ID = @CI_ID;";
                    cmd.Parameters.AddWithValue("@CI_ID", cartId);
                    row = cmd.ExecuteNonQuery();
                }
            }
            return row > 0 ? true : false;
        }

        // Retrieves all data from customer cart.
        // This is used to display in the Cart page where they can see the products the added to their cart.
        public List<UserCartViewData> UserCartDetails(int userId)
        {
            var list = new List<UserCartViewData>();

            using (var conn = new SqlConnection(SQLStr))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandText = "SELECT PROD_MAKE, PROD_MODEL, CI.CI_ID, CI.CI_QTY, P.PROD_ID, PP_AMOUNT, PROD_IMG, COUNT(P.PROD_ID) AS PRODUCT_COUNT, PQ.PQ_QTY " +
                    "FROM CART_ITEM AS CI INNER JOIN CART AS C " +
                    "ON CI.CART_ID = C.CART_ID " +
                    "INNER JOIN[USER] AS U " +
                    "ON C.USER_ID = U.USER_ID " +
                    "INNER JOIN PRODUCT AS P " +
                    "ON CI.PROD_ID = P.PROD_ID " +
                    "INNER JOIN PRODUCT_PRICE AS PP " +
                    "ON P.PROD_ID = PP.PROD_ID " +
                    "INNER JOIN PRODUCT_QUANTITY AS PQ " +
                    "ON P.PROD_ID = PQ.PROD_ID " +
                    "WHERE C.USER_ID = @USER_ID " +
                    "GROUP BY PROD_MAKE, PROD_MODEL, CI.CI_QTY, PP_AMOUNT, PROD_IMG, CI.CI_ID, P.PROD_ID, PQ.PQ_QTY;";

                    cmd.Parameters.AddWithValue("@USER_ID", userId);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            UserCartViewData data = new UserCartViewData
                            {
                                product = new Product
                                {
                                    PROD_MAKE = reader["PROD_MAKE"].ToString(),
                                    PROD_MODEL = reader["PROD_MODEL"].ToString(),
                                    PROD_IMG = reader["PROD_IMG"].ToString(),
                                    PROD_ID = Convert.ToInt32(reader["PROD_ID"])
                                },
                                price = new ProductPrice
                                {
                                    PP_PRICE = reader["PP_AMOUNT"].ToString(),
                                },
                                productCount = Convert.ToInt32(reader["PRODUCT_COUNT"]),
                                cartItem = new CartItem
                                {
                                    CI_QTY = Convert.ToInt32(reader["CI_QTY"]),
                                    CI_ID = Convert.ToInt32(reader["CI_ID"])
                                },
                                productQuantity = new ProductQuantity
                                {
                                    PQ_QTY = Convert.ToInt32(reader["PQ_QTY"])
                                }
                            };
                            list.Add(data);
                        }
                    }
                }
            }
            return list;
        }

        public int CheckCartExist(string userId)
        {
            int row = 0;
            using (var conn = new SqlConnection(SQLStr))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandText = "SELECT CART_ID FROM CART WHERE USER_ID = @USER_ID;";
                    cmd.Parameters.AddWithValue("@USER_ID", userId);
                    using(SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if(reader.Read())
                        {
                            row = (int)reader["CART_ID"];
                        }
                    }
                }
            }
            return row;
        }

        public int CreateCart(string userId)
        {
            int cartId = 0;

            // This will just return the id of an existing or non existing cart.
            int checkCartExist = CheckCartExist(userId);

          
            using (var conn = new SqlConnection(SQLStr))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    if(checkCartExist != 0)
                    {
                        cartId = checkCartExist;
                        // if exist then we return the fetched id.
                        return cartId;
                    }

                    cmd.CommandText = "INSERT INTO CART (CART_DATE_CREATED, USER_ID) " +
                        "VALUES (DEFAULT, @USER_ID); SELECT SCOPE_IDENTITY()";
                    cmd.Parameters.AddWithValue("@USER_ID", userId);
                    cartId = Convert.ToInt32(cmd.ExecuteScalar());
                
                }
            }
            return cartId;
        }


        private bool CheckCartItem(int cartId, int productId)
        {
            bool itemExists = false;
            using (var conn = new SqlConnection(SQLStr))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandText = "SELECT 1 FROM CART_ITEM WHERE CART_ID = @CART_ID AND PROD_ID = @PROD_ID";
                    cmd.Parameters.AddWithValue("@CART_ID", cartId);
                    cmd.Parameters.AddWithValue("@PROD_ID", productId);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        itemExists = reader.Read();
                    }
                }
            }
            return itemExists;
        }


        public bool CartItemInsert(int cartId, int productId, int qty)
        {
            bool itemExists = CheckCartItem(cartId, productId);

            int rowsAffected = 0;
            using (var conn = new SqlConnection(SQLStr))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandType = System.Data.CommandType.Text;

                    if (itemExists)
                    {
                        // If the item already exists in the cart, update the quantity
                        cmd.CommandText = "UPDATE CART_ITEM SET CI_QTY = CI_QTY + @CI_QTY WHERE CART_ID = @CART_ID AND PROD_ID = @PROD_ID";
                    }
                    else
                    {
                        // If the item does not exist in the cart, insert a new row
                        cmd.CommandText = "INSERT INTO CART_ITEM (CI_QTY, CART_ID, PROD_ID) VALUES (@CI_QTY, @CART_ID, @PROD_ID)";
                    }

                    cmd.Parameters.AddWithValue("@CI_QTY", qty);
                    cmd.Parameters.AddWithValue("@CART_ID", cartId);
                    cmd.Parameters.AddWithValue("@PROD_ID", productId);

                    rowsAffected = cmd.ExecuteNonQuery();
                }
            }
            return rowsAffected > 0;
        }

    }
}