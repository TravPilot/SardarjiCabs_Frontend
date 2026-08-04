
using Dapper;
using Microsoft.Data.SqlClient;
using SardarJi_Cab_Booking.Models;
using System.Data;

namespace SardarJi_Cab_Booking.Business_Layer
{
  
    public class CarService : ICarService
    {
        private readonly string _connectionString;

        public CarService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<CarModel> GetCarByIdAsync(int carId)
        {
            using var conn = new SqlConnection(_connectionString);

            var p = new DynamicParameters();
            p.Add("@CarId", carId);

            var car = await conn.QueryFirstOrDefaultAsync<CarModel>(
                "dbo.usp_CarMaster_GetById",
                p,
                commandType: CommandType.StoredProcedure);

            return car;
        }

        
    }
}
