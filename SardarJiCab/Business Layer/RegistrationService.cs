using Dapper;
using Microsoft.Data.SqlClient;
using SardarJi_Cab_Booking.Models;
using System.Data;

namespace SardarJi_Cab_Booking.Business_Layer
{
    public class RegistrationService : IRegistrationService
    {
        private readonly string _connectionString;

        public RegistrationService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<CustomerVM> CustomerSignUp(SignUpVM customer)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);

                var p = new DynamicParameters();
                p.Add("@ClientId", customer.ClientId);
                p.Add("@Email", customer.Email);
                p.Add("@Password", customer.Password);
                p.Add("@FullName", customer.FirstName);
                p.Add("@LastName", customer.LastName);
                p.Add("@Mobile", customer.MobileNo);

                var signup = await conn.QueryFirstOrDefaultAsync<CustomerVM>(
                    "dbo.customerSignup_sardarJi",  //customerSignup_TraviYoPortal
                    p,
                    commandType: CommandType.StoredProcedure);

                return signup;
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
