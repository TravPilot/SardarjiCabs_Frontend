
    using Microsoft.AspNetCore.SignalR;

    namespace SardarJi_Cab_Booking.Helper
    {
        
        public class LocationHub : Hub
        {
            public async Task JoinBookingRoom(string bookingId)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, bookingId);
            }

            public async Task LeaveBookingRoom(string bookingId)
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, bookingId);
            }

            
            public async Task SendLocation(string bookingId, double lat, double lng, double heading, double speedKmh)
            {
                await Clients.OthersInGroup(bookingId).SendAsync("ReceiveLocation", new
                {
                    bookingId,
                    lat,
                    lng,
                    heading,
                    speedKmh,
                    timestampUtc = DateTime.UtcNow
                });
            }

            public async Task SendStatus(string bookingId, string status)
            {
                await Clients.OthersInGroup(bookingId).SendAsync("ReceiveStatus", new
                {
                    bookingId,
                    status,
                    timestampUtc = DateTime.UtcNow
                });
            }
        }
    }

