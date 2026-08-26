using SardarJiCab.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SardarJiCab.BL.Interface
{
    public interface IDriverSupportBL
    {
        Task<DriverSupport> GetSupportPageAsync(int driverId);
        Task<StatusUpdateResult> SubmitTicketAsync(int driverId, string category, string subject, string message);
    }
}
