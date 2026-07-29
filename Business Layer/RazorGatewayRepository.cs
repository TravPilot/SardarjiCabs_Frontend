using Dapper;
using Microsoft.Data.SqlClient;
using SardarJi_Cab_Booking.Models;
using System.Data;

namespace SardarJi_Cab_Booking.Business_Layer
{
    public class RazorGatewayRepository : IRazorGatewayRepository
    {
        private readonly string _connectionString;
        private readonly IConfiguration _config;

        public RazorGatewayRepository(IConfiguration configuration, IConfiguration config)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _config = config;
        }
        public async Task<PaymentGatewaySettings> GetPaymentGatewaySettings(long clientId)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);

                var p = new DynamicParameters();
                p.Add("@ClientId", clientId);


                var customerProfile = await conn.QueryFirstOrDefaultAsync<PaymentGatewaySettings>(
                    "dbo.PaymentGatewaySettings",
                    p,
                    commandType: CommandType.StoredProcedure);

                return customerProfile;
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

        public async Task SaveTransactionDetails(string id, string amt, string customerSession)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);

                var p = new DynamicParameters();
                p.Add("@ClientId", Convert.ToInt64(_config["ClientId"]));
                p.Add("@OrderId", id);
                p.Add("@customerSession", customerSession);
                p.Add("@Amount", amt);

                await conn.ExecuteAsync(
                    "dbo.SaveEaseBuzzOrderIdValidate_EB",
                    p,
                    commandType: CommandType.StoredProcedure);
            }
            catch (Exception)
            {
                throw;
            }
        }


        public async Task<RazorPayVM> ValidateOrderId(string Id)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);

                var p = new DynamicParameters();
                p.Add("@OrderId", Id);


                var ValidateOrderIde = await conn.QueryFirstOrDefaultAsync<RazorPayVM>(
                    "dbo.GetEaseBuzzOrderIdValidate_EB",
                    p,
                    commandType: CommandType.StoredProcedure);

                return ValidateOrderIde;
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
