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
            await connection.OpenAsync();
            using (SqlCommand command = new SqlCommand(sqlQuery, connection))
            {
                command.Parameters.AddWithValue("@firstname", firstName);
                command.Parameters.AddWithValue("@lastname", lastName);
                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@email", email);
                command.Parameters.AddWithValue("@hash", hash);
                command.Parameters.AddWithValue("@salt", salt);
                await command.ExecuteNonQueryAsync();
            }
            await connection.CloseAsync();

        }


        return Ok("User registered");
    }

}

[ApiController]
[Route("[controller]")]
public class AuthorizationController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> AuthorizeUser([FromForm] string email, [FromForm] string password)
    {

        byte[] salt = new byte[32];
        byte[] hash = new byte[32];

        string sqlQuery =
            $@"SELECT id, firstname, lastname, username, email, hash, salt
            FROM dbo.users
            WHERE email = @email";
        using (SqlConnection connection = TestDB.GetConnection())
        {
            await connection.OpenAsync();
            using (var command = new SqlCommand(sqlQuery, connection))
            {
                command.Parameters.AddWithValue("@email", email);
                SqlDataReader reader = command.ExecuteReader();

                while (await reader.ReadAsync())
                {
                    reader.GetBytes(5, 0, hash, 0, 32);
                    reader.GetBytes(6, 0, salt, 0, 32);
                }
                await reader.CloseAsync();
                await connection.CloseAsync();
            }
        }

        byte[] inputHash = Encryption.GetHash(password, salt);

        if (inputHash == hash)
        {
            return Ok("success!");
        }

        return Unauthorized(inputHash);
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