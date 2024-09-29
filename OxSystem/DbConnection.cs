using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;

namespace OxSystem
{
    internal class DbConnection
    {
        string _connectionString = Login_.finaldbname;


        /* protected SqlConnection getConnection()
          {
              SqlConnection con = new SqlConnection();
              con.ConnectionString = "data source = DESKTOP-O3PSP7Q ; database = localpharmaflow ; integrated security = True";
              return con;
          }*/


        protected SqlConnection getConnection()
        {
            SqlConnection con = new SqlConnection();
            con.ConnectionString = "data source = AMEERPC\\SQLEXPRESS ; database = final_proj_db ; integrated security = True";
            return con;
        }
        // Azure string connection
        /*public SqlConnection getConnection()
        {
            try
            {
                SqlConnection con = new SqlConnection
                {
                    ConnectionString = $"Server=pharmaflow.database.windows.net;" +
                                       $"Initial Catalog='{_connectionString}';" +
                                       $"Persist Security Info=False;" +
                                       $"User ID=pharmaflow;" +
                                       $"Password=ameer6563_;" +
                                       $"MultipleActiveResultSets=False;" +
                                       $"Encrypt=True;" +
                                       $"TrustServerCertificate=True;" +
                                       $"Connection Timeout=30;"
                };

                // Attempt to open the connection to check if the database exists
                con.Open();
                return con;
            }
            catch (SqlException ex)
            {
                // Handle specific errors
                if (ex.Number == 4060) // Invalid database
                {
                    MessageBox.Show($"Database '{Login.dbname}' does not exist or could not be accessed.", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    MessageBox.Show($"An error occurred while connecting to the database: {ex.Message}", "Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                return null;
            }
        }*/

        public SqlConnection getConnection_()
        {
            try
            {
                SqlConnection con = new SqlConnection
                {
                    ConnectionString = $"Server=pharmaflow.database.windows.net;" +
                                      $"Initial Catalog='master';" +
                                       $"Persist Security Info=False;" +
                                       $"User ID=pharmaflow;" +
                                       $"Password=ameer6563_;" +
                                       $"MultipleActiveResultSets=False;" +
                                       $"Encrypt=True;" +
                                       $"TrustServerCertificate=True;" +
                                       $"Connection Timeout=30;"
                };

                // Attempt to open the connection to check if the database exists
                con.Open();
                return con;
            }
            catch (SqlException ex)
            {
                // Handle specific errors
                if (ex.Number == 4060) // Invalid database
                {
                    MessageBox.Show($"Database '{Login.dbname}' does not exist or could not be accessed.", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    MessageBox.Show($"An error occurred while connecting to the database: {ex.Message}", "Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                return null;
            }
        }

        public DataSet getData_(string query)
        {
            DataSet ds = new DataSet();
            using (SqlConnection con = getConnection_())
            {
                if (con != null)
                {
                    try
                    {
                        using (SqlCommand cmd = new SqlCommand(query, con))
                        {
                            SqlDataAdapter da = new SqlDataAdapter(cmd);
                            da.Fill(ds);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"An error occurred while retrieving data: {ex.Message}", "Data Retrieval Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            return ds;
        }

        public DataSet getData(string query)
        {
            DataSet ds = new DataSet();
            using (SqlConnection con = getConnection())
            {
                if (con != null)
                {
                    try
                    {
                        using (SqlCommand cmd = new SqlCommand(query, con))
                        {
                            SqlDataAdapter da = new SqlDataAdapter(cmd);
                            da.Fill(ds);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"An error occurred while retrieving data: {ex.Message}", "Data Retrieval Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            return ds;
        }


        public void setData(string query)
        {
            using (SqlConnection con = getConnection())
            {
                if (con != null)
                {
                    try
                    {
                        con.Open(); // Open the connection before executing the query

                        using (SqlCommand cmd = new SqlCommand(query, con))
                        {
                            cmd.ExecuteNonQuery(); // Execute the query
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"An error occurred while updating data: {ex.Message}", "Data Update Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    finally
                    {
                        // Ensure the connection is closed in the finally block
                        if (con.State == System.Data.ConnectionState.Open)
                        {
                            con.Close(); // Close the connection after execution
                        }
                    }
                }
            }
        }


        //public void setData(string query)
        //{
        //    using (SqlConnection con = getConnection())
        //    {
        //        if (con != null)
        //        {
        //            try
        //            {
        //                using (SqlCommand cmd = new SqlCommand(query, con))
        //                {
        //                    cmd.ExecuteNonQuery(); // Execute the query
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                MessageBox.Show($"An error occurred while updating data: {ex.Message}", "Data Update Error", MessageBoxButton.OK, MessageBoxImage.Error);
        //            }
        //        }
        //    }
        //}


        public void CreateDatabase(string databaseName)
        {
            string connectionString = "Server=pharmaflow.database.windows.net;" +
                                      "Persist Security Info=False;" +
                                      "User ID=pharmaflow;" +
                                      "Password=ameer6563_;" +
                                      "MultipleActiveResultSets=False;" +
                                      "Encrypt=True;" +
                                      "TrustServerCertificate=True;" +
                                      "Connection Timeout=200;";

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    using (SqlCommand cmd = new SqlCommand($"CREATE DATABASE [{databaseName}]", con))
                    {
                        cmd.CommandTimeout = 200; // Set timeout to 200 seconds
                        cmd.ExecuteNonQuery();
                    }

                    // Switch to the new database
                    string dbConnectionString = $"Server=pharmaflow.database.windows.net;" +
                                                $"Initial Catalog='{databaseName}';" +
                                                $"Persist Security Info=False;" +
                                                $"User ID=pharmaflow;" +
                                                $"Password=ameer6563_;" +
                                                $"MultipleActiveResultSets=False;" +
                                                $"Encrypt=True;" +
                                                $"TrustServerCertificate=True;" +
                                                $"Connection Timeout=200;";

                    using (SqlConnection conNewDb = new SqlConnection(dbConnectionString))
                    {
                        conNewDb.Open();
                        string tableCreationScript = @"
                            CREATE TABLE bills (
                                bid INT primary key identity(1,1),
                                from_ VARCHAR(100) NULL,
                                too_ VARCHAR(100) NULL,
                                Price DECIMAL NULL,
                                bdate DATE NULL,
                                billId INT NULL,
                                type VARCHAR(100) NULL,
                                by_ VARCHAR(200) NULL
                            );
                            CREATE TABLE medicinfo (
                                mid INT primary key identity(1,1),
                                mname VARCHAR(40) NULL,
                                bprice DECIMAL NULL,
                                sprice DECIMAL NULL,
                                exdate DATE NULL,
                                madate DATE NULL,
                                nummedic INT NULL,
                                sname VARCHAR(50) NULL,
                                from_ VARCHAR(100) NULL,
                                too_ VARCHAR(100) NULL,
                                billId INT NULL,
                                codeid BIGINT NULL
                            );
                            CREATE TABLE MessageNotifications (
                                id INT primary key identity(1,1),
                                message_id INT NULL,
                                sender_id VARCHAR(50) NULL,
                                receiver_id VARCHAR(50) NULL,
                                message TEXT NULL,
                                timestamp DATETIME NULL DEFAULT GETDATE()
                            );
                            CREATE TABLE storageinfo (
                                sid INT primary key identity(1,1),
                                sname VARCHAR(50) NULL,
                                slocation VARCHAR(50) NULL,
                                size BIGINT NULL
                            );
                            CREATE TABLE Suppliers (
                                sid INT primary key identity(1,1),
                                supname VARCHAR(200) NULL,
                                supnum BIGINT NULL,
                                suplocation VARCHAR(200) NULL
                            );
                            CREATE TABLE UserMessages (
                                id INT primary key identity(1,1),
                                sender_id NVARCHAR(50) NULL,
                                receiver_id NVARCHAR(50) NULL,
                                message NVARCHAR(MAX) NULL,
                                timestamp DATETIME NULL DEFAULT GETDATE()
                            );
                            CREATE TABLE users_info (
                                id INT primary key identity(1,1),
                                user_name VARCHAR(50) NULL,
                                password VARCHAR(50) NULL,
                                role VARCHAR(30) NULL,
                                email VARCHAR(40) NULL,
                                phone_num BIGINT NULL,
                                address VARCHAR(40) NULL,
                                dob DATE NULL,
                                fullname VARCHAR(50) NULL,
                                perms VARCHAR(200) NULL
                            );
                        ";

                        using (SqlCommand cmd = new SqlCommand(tableCreationScript, conNewDb))
                        {
                            cmd.CommandTimeout = 200; // Set timeout to 200 seconds
                            cmd.ExecuteNonQuery();
                        }
                    }
                }

                MessageBox.Show($"Database '{databaseName}' and tables created successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
