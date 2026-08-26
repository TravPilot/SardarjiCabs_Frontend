using SardarJiCab.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SardarjiCab.DB.Interface
{
    public interface IDriverSupportDB
    {
        Task<List<SupportTicket>> GetTicketsAsync(int driverId);
        Task<long> CreateTicketAsync(int driverId, string category, string subject, string message);
    }
}
