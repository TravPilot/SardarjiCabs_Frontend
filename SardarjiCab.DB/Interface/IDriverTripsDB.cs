using SardarJiCab.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SardarjiCab.DB.Interface
{
    public interface IDriverTripsDB
    {
        Task<List<CabBooking>> GetUpcomingTripsAsync(int driverId);
        Task<List<CabBooking>> GetCompletedTripsAsync(int driverId);
        Task<List<CabBooking>> GetOnGoingTripsAsync(int driverId);
        Task<int> StartTripAsync(long bookingId, int driverId, string enteredOtp);


        Task<CabBooking> GetActiveTripAsync(long bookingId, int driverId);
        Task<int> CompleteTripAsync(long bookingId, int driverId);
        Task UpdateLiveLocationAsync(long bookingId, int driverId, double latitude, double longitude);
    }
}
