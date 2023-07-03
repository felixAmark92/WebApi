using System.Data.SqlClient;

public static class TestDB
{
    public static SqlConnection GetConnection()
    {
        string connection = File.ReadAllText(@"C:\Users\Felix\Documents\Secure\connection.txt");
        return new SqlConnection(connection);

    }
}

