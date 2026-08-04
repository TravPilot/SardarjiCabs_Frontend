using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SardarJiCab.Model
{
    public class DriverTrips
    {
        public List<CabBooking> OnGoing { get; set; } = new();
        public List<CabBooking> UpcomingTrips { get; set; } = new();
        public List<CabBooking> CompletedTrips { get; set; } = new();
    }
}
