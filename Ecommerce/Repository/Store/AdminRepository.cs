using Ecommerce.Models;
using Ecommerce.Models.Store;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;

namespace Ecommerce.Repository.Store
{
    public class AdminRepository
    {
        private string SQLString()
        {
            return ConfigurationManager.AppSettings["SQLStr"];
        }

        public object[] AdminAuth(Credential creds)
        {
            object[] response = new object[6];
            using (var conn = new SqlConnection(SQLString()))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandText = "SELECT A.A_ID, A_FNAME, A_LNAME, C_EMAIL, C_PASS FROM CREDENTIAL AS C " +
                        "INNER JOIN ADMIN AS A " +
                        "ON C.A_ID = A.A_ID " +
                        "WHERE C_EMAIL = @C_EMAIL;";

                    cmd.Parameters.AddWithValue("@C_EMAIL", creds.C_EMAIL.Trim());

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (creds.C_PASS.Trim() == reader["C_PASS"].ToString().Trim())
                            {
                                response[0] = true;
                                response[1] = reader["C_EMAIL"].ToString().Trim();
                                response[2] = reader["C_PASS"].ToString().Trim();
                                response[3] = reader["A_ID"].ToString().Trim();
                                response[4] = reader["A_FNAME"].ToString().Trim();
                                response[5] = reader["A_LNAME"].ToString().Trim();
                            }
                        }
                    }
                }
            }
            return response[0] is true ? response : new object[] { false };
        }

        public int AdminDetails(Admin data)
        {
            int id = 0;
            try
            {
                using (var conn = new SqlConnection(SQLString()))
                {
                    conn.Open();
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandType = System.Data.CommandType.Text;
                        cmd.CommandText = "INSERT INTO ADMIN(A_FNAME, A_LNAME, A_MNAME, A_PHONE) VALUES(@fname, @lname, @mname, @phone); SELECT SCOPE_IDENTITY();";

                        cmd.Parameters.AddWithValue("@fname", data.A_FNAME);
                        cmd.Parameters.AddWithValue("@lname", data.A_LNAME);
                        cmd.Parameters.AddWithValue("@mname", data.A_MNAME);
                        cmd.Parameters.AddWithValue("@phone", data.A_PHONE);

                        id = Convert.ToInt32(cmd.ExecuteScalar());
                        return id;
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
            
        }

        public int AdminAddress(Address data)
        {
            int id = 0;
            try
            {
                using (var conn = new SqlConnection(SQLString()))
                {
                    conn.Open();
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandType = System.Data.CommandType.Text;
                        cmd.CommandText = "INSERT INTO ADDRESS(AD_STREET, AD_BRGY, AD_PROVINCE, AD_CITY, AD_ZIPCODE, A_ID) VALUES(@street, @brgy, @province, @city, @zip, @id); SELECT SCOPE_IDENTITY();";

                        cmd.Parameters.AddWithValue("@street", data.AD_STREET);
                        cmd.Parameters.AddWithValue("@brgy", data.AD_BRGY);
                        cmd.Parameters.AddWithValue("@province", data.AD_PROVINCE);
                        cmd.Parameters.AddWithValue("@city", data.AD_CITY);
                        cmd.Parameters.AddWithValue("@zip", data.AD_ZIPCODE);
                        cmd.Parameters.AddWithValue("@id", data.A_ID);

                        id = Convert.ToInt32(cmd.ExecuteScalar());
                        return id;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public int AdminCredential(Credential data)
        {
            int id = 0;
            try
            {
                using (var conn = new SqlConnection(SQLString()))
                {
                    conn.Open();
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandType = System.Data.CommandType.Text;
                        cmd.CommandText = "INSERT INTO CREDENTIAL(C_EMAIL, C_PASS, A_ID) VALUES(@email, @pass, @id); SELECT SCOPE_IDENTITY();";

                        cmd.Parameters.AddWithValue("@email", data.C_EMAIL);
                        cmd.Parameters.AddWithValue("@pass", data.C_PASS);
                        cmd.Parameters.AddWithValue("@id", data.A_ID);

                        id = Convert.ToInt32(cmd.ExecuteScalar());
                        return id;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

        }
    }
}