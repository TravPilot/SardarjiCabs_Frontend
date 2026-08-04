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
    public class DriverTripsBL: IDriverTripsBL
    {
        private readonly IDriverTripsDB _driverTripsDB;

        public DriverTripsBL(IDriverTripsDB driverTripsDB)
        {
            _driverTripsDB = driverTripsDB;
        }

        public async Task<DriverTrips> GetTripsAsync(int driverId)
        {
            var upcoming = await _driverTripsDB.GetUpcomingTripsAsync(driverId);
            var completed = await _driverTripsDB.GetCompletedTripsAsync(driverId);
            var onGoing = await _driverTripsDB.GetOnGoingTripsAsync(driverId);

            return new DriverTrips
            {
                UpcomingTrips = upcoming.ToList(),
                CompletedTrips = completed.ToList(),
                OnGoing = onGoing.ToList()
            };
        }

        public async Task<StatusUpdateResult> StartTripAsync(long bookingId, int driverId, string enteredOtp)
        {
            if (string.IsNullOrWhiteSpace(enteredOtp) || enteredOtp.Trim().Length != 6)
                return new StatusUpdateResult { Success = false, Message = "Enter the 6-digit OTP shared by the passenger." };

            var rows = await _driverTripsDB.StartTripAsync(bookingId, driverId, enteredOtp.Trim());

            return rows > 0
                ? new StatusUpdateResult { Success = true, Message = "Trip started." }
                : new StatusUpdateResult { Success = false, Message = "Incorrect OTP, please enter the correct OTP." };
        }


        public async Task<TripInProgressViewModel> GetActiveTripAsync(long bookingId, int driverId)
        {
            var row = await _driverTripsDB.GetActiveTripAsync(bookingId, driverId);
            if (row == null) return null;

            return new TripInProgressViewModel
            {
                BookingId = row.BookingId,
                BookingNo = row.BookingNo,
                PickupAddress = row.PickupAddress,
                DropAddress = row.DropAddress,
                PassengerName = row.PassengerName,
                ContactNumber = row.ContactNumber,
                NetPayable = row.NetPayable
            };
        }

        public async Task<StatusUpdateResult> CompleteTripAsync(long bookingId, int driverId)
        {
            var rows = await _driverTripsDB.CompleteTripAsync(bookingId, driverId);
            return rows > 0
                ? new StatusUpdateResult { Success = true, Message = "Trip completed." }
                : new StatusUpdateResult { Success = false, Message = "This trip can't be completed — it may not have started yet, or is already finished." };
        }

        public Task UpdateLiveLocationAsync(long bookingId, int driverId, double latitude, double longitude) =>
            _driverTripsDB.UpdateLiveLocationAsync(bookingId, driverId, latitude, longitude);
    }
}
