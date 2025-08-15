using Microsoft.Data.SqlClient;

namespace Reelography.Api.Helper;

public class SqlHelper
{
    //this field gets initialized at Program.cs
    public static string? ConnectionString;

    /// <summary>
    /// Get Connection String.
    /// </summary>
    /// <returns></returns>
    public static SqlConnection GetConnection()
    {
        try
        {
            var connection = new SqlConnection(ConnectionString);
            return connection;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}