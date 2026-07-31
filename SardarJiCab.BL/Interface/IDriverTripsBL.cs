using SardarJiCab.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SardarJiCab.BL.Interface
{
    public interface IDriverTripsBL
    {
        Task<DriverTrips> GetTripsAsync(int driverId);
        Task<StatusUpdateResult> StartTripAsync(long bookingId, int driverId, string enteredOtp);

        Task<TripInProgressViewModel> GetActiveTripAsync(long bookingId, int driverId);
        Task<StatusUpdateResult> CompleteTripAsync(long bookingId, int driverId);
        Task UpdateLiveLocationAsync(long bookingId, int driverId, double latitude, double longitude);
    }
}
