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
        byte[] salt = Encryption.GetSalt();
        byte[] hash = Encryption.GetHash(password, salt);

        string sqlQuery =
            @"INSERT INTO dbo.users (firstname, lastname, username, email, hash, salt)
            VALUES (@firstname, @lastname, @username, @email, @hash, @salt)";

        using (SqlConnection connection = TestDB.GetConnection())
        {
            connection.Open();
            using (SqlCommand command = new SqlCommand(sqlQuery, connection))
            {
                command.Parameters.AddWithValue("@firstname", firstName);
                command.Parameters.AddWithValue("@lastname", lastName);
                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@email", email);
                command.Parameters.AddWithValue("@hash", hash);
                command.Parameters.AddWithValue("@salt", salt);
                command.ExecuteNonQuery();
            }
            connection.Close();
        }

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
                currentUser.Id = reader.GetInt32(0);
                currentUser.FirstName = reader["firstname"].ToString();
                currentUser.LastName = reader["lastname"].ToString();
                currentUser.Username = reader["username"].ToString();
                currentUser.Email = reader["email"].ToString();
                reader["lastname"].ToString();
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
    [HttpGet("read")]
    public IActionResult ReadCookie()
    {
        // Read a cookie
        string? cookieValue = Request.Cookies["sessionId"];
        return Ok("Cookie value: " + cookieValue);
    }
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
    public static byte[] GetSalt()
    {
        var random = RandomNumberGenerator.Create();
        int max_length = 32;
        byte[] salt = new byte[max_length];

        random.GetNonZeroBytes(salt);

        return salt;
    }

}

public class SqlHandler
{

}