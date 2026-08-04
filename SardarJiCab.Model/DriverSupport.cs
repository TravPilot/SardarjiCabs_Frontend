using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SardarJiCab.Model
{
    public class DriverSupport
    {
        public string SupportPhone { get; set; } = "+91 99903 50250";
        public string SupportWhatsAppNumber { get; set; } = "919990350250";
        public string SupportEmail { get; set; } = "support@sardarjiev.com";

        public List<FaqItem> Faqs { get; set; } = new();
        public List<SupportTicket> Tickets { get; set; } = new();
    }

    public class FaqItem
    {
        public string Question { get; set; }
        public string Answer { get; set; }
    }

    public class SupportTicket
    {
        public long TicketId { get; set; }
        public string Category { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public string Status { get; set; } // Open, InProgress, Resolved
        public string AdminReply { get; set; }
        public DateTime CreatedAt { get; set; }

        public string StatusCss => (Status ?? "").Replace(" ", "").ToLowerInvariant();
    }
}
