using System.Data;

namespace SardarJi_Cab_Booking.Business_Layer
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}
