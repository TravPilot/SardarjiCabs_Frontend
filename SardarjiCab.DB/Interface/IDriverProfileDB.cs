using SardarJiCab.Model;
using SardarJiCab.Model.SardarJiEV.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SardarjiCab.DB.Interface
{
    public interface IDriverProfileDB
    {
        Task<DriverProfile> GetByIdAsync(int driverId);
        Task<int> UpdateProfileAsync(int driverId, string fullName, string email, string gender,
            string city, string pinCode, DateTime? dateOfBirth, string address);
        Task<int> UpdatePhotoAsync(int driverId, string photoUrl);
        Task<string> GetPasswordHashAsync(int driverId);
        Task<int> UpdatePasswordAsync(int driverId, string newPasswordHash);
    }
}
