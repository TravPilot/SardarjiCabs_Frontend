using Dapper;
using Microsoft.Data.SqlClient;
using SardarJi_Cab_Booking.Models;
using System.Data;

namespace SardarJi_Cab_Booking.Business_Layer
{
    public class LogInService : ILogInService
    {
        private readonly string _connectionString;

        public LogInService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<CustomerVM> customerLogin(CustomerVM customer)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);

                var p = new DynamicParameters();
                p.Add("@ClientId", customer.ClientId);
                p.Add("@Email", customer.UserName);
                p.Add("@Password", customer.Password);

                var login = await conn.QueryFirstOrDefaultAsync<CustomerVM>(
                    "dbo.customerLogin_TraviYoPortalNew",
                    p,
                    commandType: CommandType.StoredProcedure);

                return login;
            }
            catch (SqlException ex)
            {
                

                throw;
            }
            catch (Exception ex)
            {
                

                throw;
            }
        }
    }
}
