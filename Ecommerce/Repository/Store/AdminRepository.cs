using Ecommerce.Models;
using Ecommerce.Models.Store;
using System;
using System.Collections.Generic;
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
            return "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=D:\\Code-Space\\VisualStudio\\Ecommerce\\Ecommerce\\App_Data\\Ladaza.mdf;Integrated Security=True";
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