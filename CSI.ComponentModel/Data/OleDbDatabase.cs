using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Threading;

namespace CSI.Data
{
    /// <summary>
    /// Class that encapsulates a DB2 Database connections 
    /// and CRUD operations
    /// </summary>
    public class OleDbDatabase : IDisposable
    {
        protected DbConnection _connection;
        /// <summary>
        /// Default constructor which uses the "DefaultConnection" connectionString
        /// </summary>
        public OleDbDatabase()
            : this("DefaultConnection")
        {
        }

        /// <summary>
        /// Constructor which takes the connection string name
        /// </summary>
        /// <param name="connectionStringName"></param>
        public OleDbDatabase(string connectionStringName)
        {
            var connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            _connection = new OleDbConnection(connectionString);
        }

        /// <summary>
        /// Executes a non-query DB2 statement
        /// </summary>
        /// <param name="commandText">The Oracle query to execute</param>
        /// <param name="parameters">Optional parameters to pass to the query</param>
        /// <returns>The count of records affected by the Oracle statement</returns>
        public int Execute(string commandText, params OleDbParameter[] parameters)
        {
            int result;

            if (string.IsNullOrEmpty(commandText))
            {
                throw new ArgumentException("Command text cannot be null or empty.");
            }

            try
            {
                EnsureConnectionOpen();
                var command = CreateCommand(commandText, parameters);
                result = command.ExecuteNonQuery();
            }
            finally
            {
                _connection.Close();
            }

            return result;
        }

        /// <summary>
        /// Executes a DB2 query that returns a single scalar value as the result.
        /// </summary>
        /// <param name="commandText">The DB2 query to execute</param>
        /// <param name="parameters">Optional parameters to pass to the query</param>
        /// <returns></returns>
        public object QueryValue(string commandText, params OleDbParameter[] parameters)
        {
            object result;

            if (string.IsNullOrEmpty(commandText))
            {
                throw new ArgumentException("Command text cannot be null or empty.");
            }

            try
            {
                EnsureConnectionOpen();
                var command = CreateCommand(commandText, parameters);
                result = command.ExecuteScalar();
            }
            finally
            {
                EnsureConnectionClosed();
            }

            return result;
        }

        /// <summary>
        /// Executes a SQL query that returns a list of rows as the result.
        /// </summary>
        /// <param name="commandText">The Oracle query to execute</param>
        /// <param name="parameters">Parameters to pass to the Oracle query</param>
        /// <returns>A list of a Dictionary of Key, values pairs representing the 
        /// ColumnName and corresponding value</returns>
        public List<Dictionary<string, string>> Query(string commandText, params OleDbParameter[] parameters)
        {
            List<Dictionary<string, string>> rows;
            if (string.IsNullOrEmpty(commandText))
            {
                throw new ArgumentException("Command text cannot be null or empty.");
            }

            try
            {
                EnsureConnectionOpen();
                var command = CreateCommand(commandText, parameters);
                using (var reader = command.ExecuteReader())
                {
                    rows = new List<Dictionary<string, string>>();
                    while (reader.Read())
                    {
                        var row = new Dictionary<string, string>();
                        for (var i = 0; i < reader.FieldCount; i++)
                        {
                            var columnName = reader.GetName(i);
                            var columnValue = reader.IsDBNull(i) ? null : reader.GetValue(i).ToString();
                            row.Add(columnName, columnValue);
                        }
                        rows.Add(row);
                    }
                }
            }
            finally
            {
                EnsureConnectionClosed();
            }

            return rows;
        }

        public Dictionary<string, string> QuerySingle(string commandText, params OleDbParameter[] parameters)
        {
            if (string.IsNullOrEmpty(commandText))
            {
                throw new ArgumentException("Command text cannot be null or empty.");
            }

            var row = new Dictionary<string, string>();
            try
            {
                EnsureConnectionOpen();
                var command = CreateCommand(commandText, parameters);
                using (var reader = command.ExecuteReader(CommandBehavior.SingleResult))
                {
                    
                    while (reader.Read())
                    {
                        for (var i = 0; i < reader.FieldCount; i++)
                        {
                            var columnName = reader.GetName(i);
                            var columnValue = reader.IsDBNull(i) ? null : reader.GetValue(i).ToString();
                            row.Add(columnName, columnValue);
                        }                        
                    }                    
                }                
            }
            finally
            {
                EnsureConnectionClosed();
            }
            return row;
        }

        protected void EnsureConnectionOpen()
        {
            var retries = 3;
            if (_connection.State == ConnectionState.Open)
            {
                return;
            }
            while (retries >= 0 && _connection.State != ConnectionState.Open)
            {
                _connection.Open();
                retries--;
                Thread.Sleep(30);
            }
        }

        /// <summary>
        /// Closes a connection if open
        /// </summary>
        protected void EnsureConnectionClosed()
        {
            if (_connection.State == ConnectionState.Open)
            {
                _connection.Close();
            }
        }

        /// <summary>
        /// Creates a OracleCommand with the given parameters
        /// </summary>
        /// <param name="commandText">The Oracle query to execute</param>
        /// <param name="parameters">Parameters to pass to the Oracle query</param>
        /// <returns></returns>
        protected virtual OleDbCommand CreateCommand(string commandText, params OleDbParameter[] parameters)
        {
            var command = _connection.CreateCommand() as OleDbCommand;
            command.CommandText = commandText;
            AddParameters(command, parameters);
            return command;
        }

        /// <summary>
        /// Adds the parameters to a Oracle command
        /// </summary>
        /// <param name="command">The Oracle query to execute</param>
        /// <param name="parameters">Parameters to pass to the Oracle query</param>
        private static void AddParameters(OleDbCommand command, params OleDbParameter[] parameters)
        {
            if (parameters == null) return;

            foreach (var parameter in parameters)
            {
                command.Parameters.Add(parameter);
            }
        }

        /// <summary>
        /// Helper method to return query a string value 
        /// </summary>
        /// <param name="commandText">The DB2 query to execute</param>
        /// <param name="parameters">Parameters to pass to the Oracle query</param>
        /// <returns>The string value resulting from the query</returns>
        public string GetStrValue(string commandText, params OleDbParameter[] parameters)
        {
            var value = QueryValue(commandText, parameters) as string;
            return value;
        }

        public void Dispose()
        {
            if (_connection == null) return;

            _connection.Dispose();
            _connection = null;
        }
    }
}
