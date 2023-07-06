using System.Data.SqlClient;
using WebApi.Models;

namespace WebApi.Services;

public static class SqlHandler
{
    public static void TryExecuteNonQuery(string sqlQuery, Dictionary<string, object> parameters, string connectionString)
    {
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();
            ExecuteNonQuery(sqlQuery, parameters, connection);
            connection.Close();
        }
    }
    public static void TryExecuteNonQuery(string sqlQuery, string key, object value, string connectionString)
    {
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();
            ExecuteNonQuery(sqlQuery, key, value, connection);
            connection.Close();
        }
    }
    private static void ExecuteNonQuery(string sqlQuery, Dictionary<string, object> parameters, SqlConnection connection)
    {
        using (var command = new SqlCommand(sqlQuery, connection))
        {
            foreach (var parameter in parameters)
            {
                command.Parameters.AddWithValue(parameter.Key, parameter.Value);
            }
            command.ExecuteNonQuery();
        }
    }
    private static void ExecuteNonQuery(string sqlQuery, string key, object value, SqlConnection connection)
    {
        using (var command = new SqlCommand(sqlQuery, connection))
        {
            command.Parameters.AddWithValue(key, value);
            command.ExecuteNonQuery();
        }
    }

    public static CurrentUser CreateCurrentUser(SqlDataReader reader)
    {
        return new CurrentUser(
            reader.GetInt32(0),
            reader["firstname"].ToString(),
            reader["lastname"].ToString(),
            reader["username"].ToString(),
            reader["email"].ToString());
    }

    // Other methods for executing queries, retrieving data, etc.
}