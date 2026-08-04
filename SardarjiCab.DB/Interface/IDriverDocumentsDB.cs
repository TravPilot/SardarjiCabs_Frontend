using SardarJiCab.Model.SardarJiEV.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SardarjiCab.DB.Interface
{
    public interface IDriverDocumentsDB
    {
        Task<Driver> GetDocumentsAsync(int driverId);
        Task<int> UpdateLicenseAsync(int driverId, string licenseNumber, DateTime? expiryDate, string photoUrl);
        Task<int> UpdateAadhaarAsync(int driverId, string aadhaarNumber, string photoUrl);
        Task<int> UpdateRcAsync(int driverId, string rcNumber, string photoUrl);
    }
}
