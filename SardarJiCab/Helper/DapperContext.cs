using Microsoft.Data.SqlClient;
using System.Data;

namespace CabBookingMVC.Helper
{
    public class DapperContext
    {
        private readonly IConfiguration _configuration;

        public DapperContext()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            _configuration = builder.Build();
        }

        public IDbConnection CreateConnection()
        {
            return new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        }
    }
}
