using SardarJiCab.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SardarJiCab.BL.Interface
{
    public interface IDriverDashboardBL
    {
        Task<DriverDashboard> GetDashboardAsync(int driverId);
        Task<StatusUpdateResult> SetOnlineStatusAsync(int driverId, bool isOnline);
    }
}
