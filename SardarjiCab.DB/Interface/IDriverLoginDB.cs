using SardarJiCab.Model.SardarJiEV.Models;

namespace SardarjiCab.DB.Interface
{
    public interface IDriverLoginDB
    {
        Task<Driver> GetDriverByMobileAsync(string mobile);
        Task UpdateLastLoginAsync(Int64 driverId, string ipAddress);
        Task RecordFailedLoginAsync(Int64 driverId, int failedAttempts, DateTime? lockedUntil);
    }
}
