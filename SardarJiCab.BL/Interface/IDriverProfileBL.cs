using SardarJiCab.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SardarJiCab.BL.Interface
{
    public interface IDriverProfileBL
    {
        Task<DriverProfile> GetProfileAsync(int driverId);
        Task<StatusUpdateResult> UpdateProfileAsync(int driverId, string fullName, string email, string gender,
            string city, string pinCode, DateTime? dateOfBirth, string address);
        Task<StatusUpdateResult> UpdatePhotoAsync(int driverId, string photoUrl);
        Task<StatusUpdateResult> ChangePasswordAsync(int driverId, string currentPassword, string newPassword);
    }
}
