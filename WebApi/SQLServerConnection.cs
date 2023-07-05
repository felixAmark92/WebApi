using System.Data.SqlClient;

public static class TestDB
{
    public static readonly string ConnectionString = File.ReadAllText(@"C:\Users\Felix\Documents\Secure\connection.txt");
    public static SqlConnection GetConnection()
    {
        return new SqlConnection(ConnectionString);
    }


}

