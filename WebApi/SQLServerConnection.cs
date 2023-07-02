using System.Data.SqlClient;

public static class TestDB
{
    private static string connection = File.ReadAllText(@"C:\Users\Felix\Documents\Secure\connection.txt");
    public static SqlConnection Connection = new SqlConnection(connection);
}

