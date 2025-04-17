using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ConnectionLayer
{
    public static class clsLocation
    {


        public static async Task<string?> GetCountryCode(string countryName)
        {
            string query = "SELECT top 1  CountryCode FROM Countries WHERE CountryName = @CountryName";
            string? countryCode = "";

            using (SqlConnection connection = new SqlConnection(clsConnectionGenral.ConnectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@CountryName", countryName);

                try
                {
                    connection.Open();
                    object? result = await command.ExecuteScalarAsync();

                    if (result != null && result != DBNull.Value)
                    {
                        countryCode = result.ToString();

                        return countryCode;
                    }
                }
                catch (Exception ex)
                {
                    // Handle exception or rethrow
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }

            return null;
        }


        public static async Task<List<string>?> GetAllCountries()
        {
            string query = "SELECT CountryName FROM Countries";

            List<string> Countries = new List<string>();

            using (SqlConnection connection = new SqlConnection(clsConnectionGenral.ConnectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {

                try
                {
                    connection.Open();
                    using (SqlDataReader Reader =await command.ExecuteReaderAsync()) {

                        while (Reader.Read()) {
                            if (Reader["CountryName"] != null) {
                                Countries.Add((Reader["CountryName"].ToString()));
                            
                            }
                        }
                    
                    }
            
                }
                catch (Exception ex)
                {
                    // Handle exception or rethrow
                    Console.WriteLine($"Error: {ex.Message}");
                    return null;
                }
            }

            return Countries;
        }


    }
}
