using SardarJiCab.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SardarJiCab.BL.Interface
{
    public interface IDriverDocumentsBL
    {
        Task<DriverDocuments> GetDocumentsAsync(int driverId);
        Task<StatusUpdateResult> UpdateLicenseAsync(int driverId, string licenseNumber, DateTime? expiryDate, string photoUrl);
        Task<StatusUpdateResult> UpdateAadhaarAsync(int driverId, string aadhaarNumber, string photoUrl);
        Task<StatusUpdateResult> UpdateRcAsync(int driverId, string rcNumber, string photoUrl);
    }
}
