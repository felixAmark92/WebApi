namespace WebApi.Models;

public class CurrentUser
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public CurrentUser()
    { }

    public CurrentUser(int id, string firstName, string lastName, string username, string email)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        Username = username;
        Email = email;
    }
}