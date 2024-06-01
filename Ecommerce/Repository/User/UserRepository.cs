using Ecommerce.Models;
using Ecommerce.Models.User;
using Ecommerce.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ecommerce.Repository.User
{
    public class UserRepository
    {
        private UserViewData viewDataModel;
        public UserRepository(UserViewData model=null) { 
            viewDataModel = model;
        }

        private string SQLString()
        {
            return ConfigurationManager.AppSettings["SQLStr"];
        }

        public object[] BillingAddress(int id)
        {
            object[] response = new object[6];
            using (var conn = new SqlConnection(SQLString()))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandText = "SELECT AD_STREET, AD_BRGY, AD_CITY, AD_PROVINCE, AD_ZIPCODE, USER_PHONE " +
                        "FROM ADDRESS AS A " +
                        "INNER JOIN " +
                        "[USER] AS U ON A.USER_ID = @USER_ID;";

                    cmd.Parameters.AddWithValue("@USER_ID", id);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            response[0] = reader["AD_STREET"];
                            response[1] = reader["AD_BRGY"];
                            response[2] = reader["AD_CITY"];
                            response[3] = reader["AD_PROVINCE"];
                            response[4] = reader["AD_ZIPCODE"];
                            response[5] = reader["USER_PHONE"];
                        }
                        return response;
                    }
                }
            }
        }

        public object[] UserLogin(Credential cred)
        {
            object[] response = new object[6];
            using(var conn = new SqlConnection(SQLString()))
            {
                conn.Open();
                using(var cmd = conn.CreateCommand())
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandText = "SELECT U.USER_ID, USER_FNAME, USER_LNAME, C_EMAIL, C_PASS FROM CREDENTIAL AS C " +
                        "INNER JOIN [USER] AS U" +
                        " ON C.USER_ID = U.USER_ID" +
                        " WHERE C_EMAIL = @C_EMAIL;";
                    cmd.Parameters.AddWithValue("@C_EMAIL", cred.C_EMAIL);
                    using(SqlDataReader reader =  cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (cred.C_PASS == reader["C_PASS"].ToString().Trim())
                            {
                                response[0] = true;
                                response[1] = reader["C_EMAIL"].ToString().Trim();
                                response[2] = reader["C_PASS"].ToString().Trim();
                                response[3] = reader["USER_ID"].ToString().Trim();
                                response[4] = reader["USER_FNAME"].ToString().Trim();
                                response[5] = reader["USER_LNAME"].ToString().Trim();
                                return response;
                            }
                        }
                    }
                }
            }
            response.Append(false);
            return response;
        }
         
        public bool[] UserRegistrationInsert()
        {
            bool IsRegSuccess = true;
            SqlTransaction transaction = null;
            try
            {
                using (var conn = new SqlConnection(SQLString()))
                {
                    conn.Open();
                    transaction = conn.BeginTransaction();
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.Transaction = transaction;
                        cmd.CommandType = System.Data.CommandType.Text;
                        cmd.CommandText = "INSERT INTO [USER](USER_FNAME, USER_LNAME, USER_PHONE)" +
                            "VALUES(@USER_FNAME, @USER_LNAME, @USER_PHONE); SELECT SCOPE_IDENTITY()";

                        cmd.Parameters.AddWithValue("@USER_FNAME", viewDataModel.UserModel.USER_FNAME);
                        cmd.Parameters.AddWithValue("@USER_LNAME", viewDataModel.UserModel.USER_LNAME);
                        cmd.Parameters.AddWithValue("@USER_PHONE", viewDataModel.UserModel.USER_PHONE);
                       
                        int id = Convert.ToInt32(cmd.ExecuteScalar());
                        
                        if(id < 1)
                        {
                            transaction.Rollback();
                            IsRegSuccess = false;
                            return new bool[] { IsRegSuccess, false, false };
                        }

                        bool address = UserAddress(id, transaction);
                        bool credential = UserCredential(id, transaction);

                        if(!IsRegSuccess || !address || !credential)
                        {
                            IsRegSuccess = false;
                            transaction.Rollback();
                        }
                        else
                        {
                            transaction.Commit();
                        }

                        return new bool[] { IsRegSuccess, address, credential };
                    }
                }
            }
            catch (Exception e)
            {
                if (transaction != null && transaction.Connection != null)
                {
                    transaction.Rollback();
                }
                throw e;
            }
        }

        public bool UserAddress(int data, SqlTransaction transaction)
        {
            try
            {
                using (var cmd = transaction.Connection.CreateCommand())
                {
                    cmd.Transaction = transaction;
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandText = "INSERT INTO ADDRESS(AD_STREET, AD_BRGY, AD_PROVINCE, AD_CITY, AD_ZIPCODE, USER_ID) VALUES(@street, @brgy, @province, @city, @zip, @id); SELECT SCOPE_IDENTITY();";

                    cmd.Parameters.AddWithValue("@street", viewDataModel.Address.AD_STREET);
                    cmd.Parameters.AddWithValue("@brgy", viewDataModel.Address.AD_BRGY);
                    cmd.Parameters.AddWithValue("@province", viewDataModel.Address.AD_PROVINCE);
                    cmd.Parameters.AddWithValue("@city", viewDataModel.Address.AD_CITY);
                    cmd.Parameters.AddWithValue("@zip", viewDataModel.Address.AD_ZIPCODE);
                    cmd.Parameters.AddWithValue("@id", data);
                    int row = cmd.ExecuteNonQuery();
                    return row > 0 ? true : false;
                }
            }
            catch (Exception e)
            {
                if (transaction != null && transaction.Connection != null)
                {
                    transaction.Rollback();
                }
                throw e;
            }
        }


        public bool UserCredential(int data, SqlTransaction transaction)
        {
            try
            {
                using (var cmd = transaction.Connection.CreateCommand())
                {
                    cmd.Transaction = transaction;
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandText = "INSERT INTO CREDENTIAL(C_EMAIL, C_PASS, USER_ID) VALUES(@email, @pass, @id); SELECT SCOPE_IDENTITY();";

                    cmd.Parameters.AddWithValue("@email", viewDataModel.Credential.C_EMAIL);
                    cmd.Parameters.AddWithValue("@pass", viewDataModel.Credential.C_PASS);
                    cmd.Parameters.AddWithValue("@id", data);
                    int row = cmd.ExecuteNonQuery();
                    return row > 0 ? true : false;
                }
            }
            catch (Exception e)
            {
                if (transaction != null && transaction.Connection != null)
                {
                    transaction.Rollback();
                }
                throw e;
            }
        }
    }
}
