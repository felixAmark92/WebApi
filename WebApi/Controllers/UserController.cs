using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using WebApi.Models;
using WebApi.Services;

namespace WebApi.Controllers;


[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    [HttpPost]
    public IActionResult CreateUser(
        [FromForm] string firstName,
        [FromForm] string lastName,
        [FromForm] string username,
        [FromForm] string email,
        [FromForm] string password)
    {
        byte[] salt = Encryption.GenerateSalt();
        byte[] hash = Encryption.CreateSHA256Hash(password, salt);

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

        byte[] inputHash = Encryption.CreateSHA256Hash(password, salt);

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
                Domain = "localhost",
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

        if (sessionId == null)
        {
            return BadRequest("no session could be found");
        }
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

    [HttpPost("EndSession")]
    public IActionResult EndSession()
    {
        string? sessionId = Request.Cookies["sessionId"];
        if (string.IsNullOrEmpty(sessionId))
        {
            return BadRequest("no session sent");
        }

        string sqlQuery = @"DELETE FROM dbo.session_ids WHERE id=@sessionId";
        SqlHandler.TryExecuteNonQuery(sqlQuery, "@sessionId", sessionId, TestDB.ConnectionString);
        Response.Cookies.Append("sessionId", "", new Microsoft.AspNetCore.Http.CookieOptions
        {
            MaxAge = TimeSpan.FromTicks(1),
            Path = "/",
            Domain = "localhost",
            Secure = true,
            HttpOnly = true
        });

        return Ok("session terminated");
    }

}
