using System.Net;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using WebApi.Models;
namespace WebApi.Controllers;


[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateUser(
        [FromForm] string firstName,
        [FromForm] string lastName,
        [FromForm] string username,
        [FromForm] string email,
        [FromForm] string password)
    {
        byte[] salt = Encryption.GenerateSalt();
        byte[] hash = Encryption.GetHash(password, salt);

        var data = new Dictionary<string, object>()
        {
            {"@firstname", firstName},
            {"@lastname", lastName},
            {"@username", username},
            {"@email", email},
            {"@hash", hash},
            {"@salt", salt}
        };
        string sqlQuery =
            @"INSERT INTO dbo.users (firstname, lastname, username, email, hash, salt)
            VALUES (@firstname, @lastname, @username, @email, @hash, @salt)";
        SqlHandler.TryExecuteNonQuery(sqlQuery, data, TestDB.ConnectionString);

        return Ok("User registered");
    }



    [HttpPost("Authorization")]
    public IActionResult AuthorizeUser([FromForm] string email, [FromForm] string password)
    {

        byte[] salt = new byte[32];
        byte[] hash = new byte[32];

        var currentUser = new CurrentUser();

        string sqlQuery =
            @"SELECT id, firstname, lastname, username, email, hash, salt
                FROM dbo.users
                WHERE email = @email";

        using SqlConnection connection = TestDB.GetConnection();

        connection.Open();
        using (var command = new SqlCommand(sqlQuery, connection))
        {
            command.Parameters.AddWithValue("@email", email);
            SqlDataReader reader = command.ExecuteReader();

            if (!reader.HasRows)
                return Unauthorized("Email or password is wrong");
            while (reader.Read())
            {
                reader.GetBytes(5, 0, hash, 0, 32);
                reader.GetBytes(6, 0, salt, 0, 32);
                currentUser = SqlHandler.CreateCurrentUser(reader);
            }
            reader.Close();
        }

        byte[] inputHash = Encryption.GetHash(password, salt);

        if (inputHash.SequenceEqual(hash))
        {
            string sqlQuery2 =
            @"INSERT INTO dbo.session_ids (id, user_id)
                VALUES (@id, @userid)";
            Guid sessionIdentifier = Guid.NewGuid();
            using (var command = new SqlCommand(sqlQuery2, connection))
            {
                command.Parameters.AddWithValue("@userid", currentUser.Id);
                command.Parameters.AddWithValue("@id", sessionIdentifier);
                command.ExecuteNonQuery();
            }
            Response.Cookies.Append("sessionId", sessionIdentifier.ToString(), new Microsoft.AspNetCore.Http.CookieOptions
            {
                Expires = DateTime.Now.AddDays(7),
                Path = "/",
                Secure = true,
                HttpOnly = true
            });
            connection.Close();
            return Ok(currentUser);
        }
        connection.Close();
        return Unauthorized("Email or password is wrong");
    }
    [HttpPost("TryGetSession")]
    public IActionResult ReadCookie()
    {
        CurrentUser? currentUser = null;
        string? sessionId = Request.Cookies["sessionId"];
        string sqlQuery =
          @"SELECT dbo.users.id, dbo.users.firstname, dbo.users.lastname, dbo.users.username, dbo.users.email
            FROM dbo.session_ids
            JOIN dbo.users ON dbo.session_ids.user_id = dbo.users.id
            WHERE dbo.session_ids.id = @sessionId";

        using SqlConnection connection = TestDB.GetConnection();

        connection.Open();
        using (var command = new SqlCommand(sqlQuery, connection))
        {
            command.Parameters.AddWithValue("@sessionId", sessionId);
            SqlDataReader reader = command.ExecuteReader();

            if (!reader.HasRows)
                return Unauthorized("no session found");
            while (reader.Read())
            {
                currentUser = SqlHandler.CreateCurrentUser(reader);
            }
            reader.Close();
        }
        connection.Close();
        if (currentUser != null)
        {
            return Ok(currentUser);
        }
        return Unauthorized("no session found");
    }


    public static class Encryption
    {
        public static byte[] GetHash(string inputString, byte[] salt)
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(inputString);
            byte[] saltedInputBytes = inputBytes.Concat(salt).ToArray();

            using (HashAlgorithm algorithm = SHA256.Create())
                return algorithm.ComputeHash(saltedInputBytes);
        }
        public static byte[] GenerateSalt()
        {
            var random = RandomNumberGenerator.Create();
            int max_length = 32;
            byte[] salt = new byte[max_length];

            random.GetNonZeroBytes(salt);

            return salt;
        }

    }

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
        public static void ExecuteNonQuery(string sqlQuery, Dictionary<string, object> parameters, SqlConnection connection)
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
}
