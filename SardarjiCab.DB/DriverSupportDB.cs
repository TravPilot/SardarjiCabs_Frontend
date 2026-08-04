using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using SardarjiCab.DB.Interface;
using SardarJiCab.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SardarjiCab.DB
{
    public class DriverSupportDB : IDriverSupportDB
    {
        private readonly string _connectionString;

        public DriverSupportDB(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<List<SupportTicket>> GetTicketsAsync(int driverId)
        {
            var tickets = new List<SupportTicket>();

            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("Driver_GetSupportTickets", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.Add(new SqlParameter("@DriverId", SqlDbType.Int) { Value = driverId });

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                tickets.Add(new SupportTicket
                {
                    TicketId = (long)reader["TicketId"],
                    Category = reader["Category"] as string,
                    Subject = reader["Subject"] as string,
                    Message = reader["Message"] as string,
                    Status = reader["Status"] as string,
                    AdminReply = reader["AdminReply"] as string,
                    CreatedAt = (DateTime)reader["CreatedOn"]
                });
            }
            return tickets;
        }

        public async Task<long> CreateTicketAsync(int driverId, string category, string subject, string message)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("Driver_CreateSupportTicket", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.Add(new SqlParameter("@DriverId", SqlDbType.Int) { Value = driverId });
            cmd.Parameters.Add(new SqlParameter("@Category", SqlDbType.NVarChar, 50) { Value = category });
            cmd.Parameters.Add(new SqlParameter("@Subject", SqlDbType.NVarChar, 150) { Value = subject });
            cmd.Parameters.Add(new SqlParameter("@Message", SqlDbType.NVarChar, 1000) { Value = message });
            var newId = new SqlParameter("@NewTicketId", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
            cmd.Parameters.Add(newId);

            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
            return (long)newId.Value;
        }
    }
}
