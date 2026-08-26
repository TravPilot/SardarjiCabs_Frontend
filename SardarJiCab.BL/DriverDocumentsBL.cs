using SardarjiCab.DB.Interface;
using SardarJiCab.BL.Interface;
using SardarJiCab.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SardarJiCab.BL
{
    public class DriverDocumentsBL: IDriverDocumentsBL
    {
        private readonly IDriverDocumentsDB _driverDocumentsDB;

        public DriverDocumentsBL(IDriverDocumentsDB driverDocumentsDB)
        {
            _driverDocumentsDB = driverDocumentsDB;
        }

        public async Task<DriverDocuments> GetDocumentsAsync(int driverId)
        {
            var driver = await _driverDocumentsDB.GetDocumentsAsync(driverId);
            if (driver == null) return null;

            return new DriverDocuments
            {
                DriverId = driverId,
                ApprovalStatus = driver.ApprovalStatus,
                RejectionReason = driver.RejectionReason,
                LicenseNumber = driver.LicenseNumber,
                LicenseExpiryDate = driver.LicenseExpiryDate,
                LicensePhotoUrl = driver.LicensePhotoUrl,
                AadhaarNumber = driver.AadhaarNumber,
                AadhaarPhotoUrl = driver.AadhaarPhotoUrl,
                RcNumber = driver.RcNumber,
                RcPhotoUrl = driver.RcPhotoUrl
            };
        }

        public async Task<StatusUpdateResult> UpdateLicenseAsync(int driverId, string licenseNumber, DateTime? expiryDate, string photoUrl)
        {
            var rows = await _driverDocumentsDB.UpdateLicenseAsync(driverId, licenseNumber, expiryDate, photoUrl);
            return rows > 0
                ? new StatusUpdateResult { Success = true, Message = "License uploaded." }
                : new StatusUpdateResult { Success = false, Message = "Could not save the license. Please try again." };
        }

        public async Task<StatusUpdateResult> UpdateAadhaarAsync(int driverId, string aadhaarNumber, string photoUrl)
        {
            var rows = await _driverDocumentsDB.UpdateAadhaarAsync(driverId, aadhaarNumber, photoUrl);
            return rows > 0
                ? new StatusUpdateResult { Success = true, Message = "Aadhaar uploaded." }
                : new StatusUpdateResult { Success = false, Message = "Could not save Aadhaar. Please try again." };
        }

        public async Task<StatusUpdateResult> UpdateRcAsync(int driverId, string rcNumber, string photoUrl)
        {
            var rows = await _driverDocumentsDB.UpdateRcAsync(driverId, rcNumber, photoUrl);
            return rows > 0
                ? new StatusUpdateResult { Success = true, Message = "RC uploaded." }
                : new StatusUpdateResult { Success = false, Message = "Could not save the RC. Please try again." };
        }
    }
}
