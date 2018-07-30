using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace Venus.AI.WebApi.Models.Utils
{
    public class DBClient
    {
        private static string _connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=userContextDb;Integrated Security=True";
        private static SqlConnection _connection = null;

        public static void Connect()
        {
            if (_connection != null)
                return;

            _connection = new SqlConnection(_connectionString);
            try
            {
                _connection.Open();
            }
            catch (Exception ex)
            {
                throw new Exception($"Can not connect to {_connectionString} : {ex.Message}");
            }
        }
        public static async Task ConnectAsync()
        {
            _connection = new SqlConnection(_connectionString);
            try
            {
                await _connection.OpenAsync();
            }
            catch(Exception ex)
            {
                throw new Exception($"Can not connect to {_connectionString} : {ex.Message}");
            }
        }
        
        public static void InsertOrUpedateContext(UserContext userContext)
        {
            string sqlExpression = $"SELECT * FROM ContextTable WHERE Id={userContext.Id}";
            SqlCommand sqlCommand = new SqlCommand(sqlExpression, _connection);
            SqlDataReader reader = sqlCommand.ExecuteReader();

            if (reader.HasRows) // если есть данные
            {
                reader.Close();
                sqlExpression = $"UPDATE ContextTable SET IntentContext='{userContext.IntentContext}', TalkContext='{userContext.TalkContext}' WHERE Id={userContext.Id}";
                sqlCommand = new SqlCommand(sqlExpression, _connection);
                sqlCommand.ExecuteNonQuery();
            }
            else
            {
                reader.Close();
                sqlExpression = $"INSERT INTO ContextTable (Id, IntentContext, TalkContext) VALUES ({userContext.Id}, '{userContext.IntentContext}', '{userContext.TalkContext}')";
                sqlCommand = new SqlCommand(sqlExpression, _connection);
                sqlCommand.ExecuteNonQuery();
            }
        }

        public static UserContext GetContext(long id)
        {
            string sqlExpression = $"SELECT * FROM ContextTable WHERE Id={id}";
            SqlCommand sqlCommand = new SqlCommand(sqlExpression, _connection);
            SqlDataReader reader = sqlCommand.ExecuteReader();
            if (reader.HasRows) // если есть данные
            {
                while (reader.Read()) // построчно считываем данные
                {
                    UserContext userContext = new UserContext();
                    userContext.Id = reader.GetInt32(0);
                    userContext.IntentContext = reader.GetString(1);
                    userContext.TalkContext = reader.GetString(2);
                    reader.Close();
                    return userContext;
                }
            }
            return null;
        }
    }
}
