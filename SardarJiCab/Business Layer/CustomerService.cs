using Dapper;
using Microsoft.Data.SqlClient;
using SardarJi_Cab_Booking.Models;
using System.Data;

namespace SardarJi_Cab_Booking.Business_Layer
{
    public class CustomerService: ICustomerService
    {
        private readonly string _connectionString;

        public CustomerService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

       

        public async Task<CustomerProfile> GetCustomerProfile(long Id)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);

                var p = new DynamicParameters();
                p.Add("@CustomerId", Id);
              

                var customerProfile = await conn.QueryFirstOrDefaultAsync<CustomerProfile>(
                    "dbo.customerProfile_TraviYoPortal",
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

        public async Task<List<CitiesVM>> GetCities(string countryId)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);

                var p = new DynamicParameters();
                p.Add("@CountryId", countryId);

                var cities = await conn.QueryAsync<CitiesVM>(
                    "dbo.Usp_GetCities",
                    p,
                    commandType: CommandType.StoredProcedure);

                return cities.ToList();
            }
            catch (SqlException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<StatesVM>> GetStates(string countryId)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);

                var p = new DynamicParameters();
                p.Add("@CountryId", countryId);

                var states = await conn.QueryAsync<StatesVM>(
                    "dbo.Usp_GetStates",
                    p,
                    commandType: CommandType.StoredProcedure);

                return states.ToList();
            }
            catch (SqlException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }


        public async Task<ProfileUpdateResult> UpdateProfileFields(long customerId, long clientId, ProfileUpdateRequest profile)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);

                var p = new DynamicParameters();
                p.Add("@Id", customerId);
                p.Add("@ClientId", clientId);
                p.Add("@Title", profile.Title);
                p.Add("@FirstName", profile.FirstName);
                p.Add("@LastName", profile.LastName);
                p.Add("@Address", profile.Address);
                p.Add("@PinCode", profile.PinCode);

                var result = await conn.QueryFirstOrDefaultAsync<ProfileUpdateResult>(
                    "dbo.Customer_UpdateProfileFields_SardarJi",
                    p,
                    commandType: CommandType.StoredProcedure);

                return result;
            }
            catch (SqlException)
            {
                throw;
            }
        }
        public async Task<CustomerProfile> ForgotPassword(CustomerProfile profile)
        {
            try
            {

                using var conn = new SqlConnection(_connectionString);

                var p = new DynamicParameters();
                p.Add("@ClientId", profile.ClientId);
                p.Add("@Email", profile.Email);
                p.Add("@WebsiteUrl", profile.WebsiteUrl);


                var customerProfile = await conn.QueryFirstOrDefaultAsync<CustomerProfile>(
                    "dbo.ForgotPassword_ForgotPortal",
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

        public async Task<QuotationEmailSettings> GetQuotationDetails(long Id)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);

                var p = new DynamicParameters();
                p.Add("@ClientId", Id);
               

                var customerProfile = await conn.QueryFirstOrDefaultAsync<QuotationEmailSettings>(
                    "dbo.GetQuotationEmailSettings_ForFrontend",
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


        public async Task<WalletVM> GetWalletDetails(long Id)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);

                var p = new DynamicParameters();
                p.Add("@CustomerId", Id);


                var customerProfile = await conn.QueryFirstOrDefaultAsync<WalletVM>(
                    "dbo.GetWalletBalanceDetails",
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


        public async Task<ProfileSettings> GetProfileSettings(long clientId)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);

                var p = new DynamicParameters();
                p.Add("@ClientId", clientId);


                var customerProfile = await conn.QueryFirstOrDefaultAsync<ProfileSettings>(
                    "dbo.GetDetailsForProfileSetting",
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
    }
}
