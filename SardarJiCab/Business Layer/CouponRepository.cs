using Dapper;
using Microsoft.AspNetCore.Connections;
using SardarJi_Cab_Booking.Models;

namespace SardarJi_Cab_Booking.Business_Layer
{
    public class CouponRepository : ICouponRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public CouponRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task RedeemAsync(int couponId, CouponUsage usage)
        {
            using var conn = _connectionFactory.CreateConnection();
            conn.Open();
            using var transaction = conn.BeginTransaction();
            try
            {
                const string incrementSql = "UPDATE Coupons SET CurrentUsageCount = CurrentUsageCount + 1 WHERE Id = @Id";
                await conn.ExecuteAsync(incrementSql, new { Id = couponId }, transaction);

                const string insertUsageSql = @"
                    INSERT INTO CouponUsages (CouponId, UserId, BookingId, DiscountAmount, UsedAt)
                    VALUES (@CouponId, @UserId, @BookingId, @DiscountAmount, SYSUTCDATETIME());";
                await conn.ExecuteAsync(insertUsageSql, usage, transaction);

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<int> GetUserUsageCountAsync(int couponId, string userId)
        {
            const string sql = "SELECT COUNT(1) FROM CabCouponUsages WHERE CouponId = @CouponId AND UserId = @UserId";
            using var conn = _connectionFactory.CreateConnection();
            return await conn.ExecuteScalarAsync<int>(sql, new { CouponId = couponId, UserId = userId });
        }
        public async Task<Coupon?> GetByCodeAsync(string code)
        {
            const string sql = "SELECT * FROM CabCoupons WHERE UPPER(Code) = UPPER(@Code)";
            using var conn = _connectionFactory.CreateConnection();
            return await conn.QueryFirstOrDefaultAsync<Coupon>(sql, new { Code = code });
        }

        public async Task<List<Coupon>> GetActiveCouponsAsync()
        {
            const string sql = @"
                SELECT * FROM CabCoupons
                WHERE IsActive = 1
                  AND SYSUTCDATETIME() BETWEEN ValidFrom AND ValidTo
                  AND (TotalUsageLimit IS NULL OR CurrentUsageCount < TotalUsageLimit)
                ORDER BY DiscountValue DESC;";

            using var conn = _connectionFactory.CreateConnection();
            var result = await conn.QueryAsync<Coupon>(sql);
            return result.ToList();
        }
    }
}
