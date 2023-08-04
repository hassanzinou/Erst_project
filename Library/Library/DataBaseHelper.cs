using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    public class DataBaseHelper
    {
        public string connectionstring = "";
        public SqlConnection connection;
        public SqlCommand command;

        public OdbcConnection odbcConnection;
        public OdbcCommand odbcCommand;


        /// <summary>
        ///   Creates new instance of DBHelper and creates the connectionstring.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <param name="server"></param>
        /// <param name="database"></param>
        /// <param name="trustedConnection"></param>
        public DataBaseHelper(string user, string password, string server, string database, bool trustedConnection,bool enableMars)
        {
            if (!trustedConnection)
            {
                this.connectionstring = "Server=" + server + "; Database=" + database + ";User Id=" + user + "; Password=" + password+";";
            }
            else
            {
                this.connectionstring = "Server=" + server + "; Database=" + database + "; Trusted_Connection=True;";
                if (enableMars)
                    this.connectionstring += " MultipleActiveResultSets=true;";
            }
        }


        /// <summary>
        ///   Creates new instance of DBHelper and creates the connectionstring for ODBC.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <param name="dsn"></param>

        public DataBaseHelper(string user, string password, string dsn)
        {
            this.connectionstring = "DSN=" + dsn + "; Uid=" + user + "; Pwd=" + password + ";";
        }
        public DataBaseHelper() { 
        }

        public bool Connect()
        {
            try
            {
                if (connection == null)
                {
                    this.connection = new SqlConnection(connectionstring);
                    this.connection.Open();
                }
                else if (this.connection.State != System.Data.ConnectionState.Open)
                {
                    this.connection.Open();
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool ConnectODBC()
        {
            try
            {
                if (connection == null)
                {
                    this.odbcConnection = new OdbcConnection(connectionstring);
                    this.odbcConnection.Open();
                }
                else if (this.odbcConnection.State != System.Data.ConnectionState.Open)
                {
                    this.odbcConnection.Open();
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public void ExcequteQuery(string query)
        {
            this.command = connection.CreateCommand();
            this.command.CommandText = query;
            this.command.ExecuteNonQuery();
        }

        public void ExcequteQueryODBC(string query)
        {
            this.odbcCommand = odbcConnection.CreateCommand();
            this.odbcCommand.CommandText = query;
            this.odbcCommand.ExecuteNonQuery();
        }

        public async Task<int> ExcequteQueryODBCAsyncTransactionBased(string query)
        {
            int result = 0;
            await Task.Run(() =>
            {
                using (OdbcConnection conn = new OdbcConnection(this.connectionstring))
                {
                    conn.Open();
                    OdbcTransaction transaction = conn.BeginTransaction();
                    try
                    {
                        this.odbcCommand = conn.CreateCommand();
                        this.odbcCommand.Transaction = transaction;
                        this.odbcCommand.CommandText = query;
                        this.odbcCommand.ExecuteNonQuery();
                        transaction.Commit();
                    
                        result = 1;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        
                        result = 0;
                    }
                    conn.Close();
                }
            });
            return result;
        }

        public int ExcequteQueryODBCTransactionBasedAsync(string query)
        {
            int result = 0;
            using (OdbcConnection conn = new OdbcConnection(this.connectionstring))
            {
                conn.Open();
                OdbcTransaction transaction = conn.BeginTransaction();
                try
                {
                    this.odbcCommand = conn.CreateCommand();
                    this.odbcCommand.Transaction = transaction;
                    this.odbcCommand.CommandText = query;
                    this.odbcCommand.ExecuteNonQuery();
                    transaction.Commit();

                    result = 1;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();

                    result = 0;
                }
                conn.Close();
                return result;
            }

        }

        public void Close()
        { 
            this.connection.Close();    
        }

        public void CloseODBC()
        {
            this.odbcConnection.Close();
        }

    }
}
