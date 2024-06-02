using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Ecommerce.Repository
{
    public class Role
    {
        private string SQLString()
        {
            return ConfigurationManager.AppSettings["SQLStr"];
        }
        public string GetUserRole(string email)
        {
            using (var conn = new SqlConnection(SQLString()))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandText = "SELECT 'User' as Role FROM [USER] WHERE Email = @Email UNION SELECT 'Admin' as Role FROM [ADMIN] WHERE Email = @Email";
                    cmd.Parameters.AddWithValue("@Email", email);

                    var role = cmd.ExecuteScalar() as string;
                    return role;
                }
            }
        }

    }
}