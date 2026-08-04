using SardarJiCab.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SardarJiCab.BL.Interface
{
    public interface IDriverEarningsBL
    {
        Task<DriverEarnings> GetEarningsAsync(int driverId, string period);
    }
}
