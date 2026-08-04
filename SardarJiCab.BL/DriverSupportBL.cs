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
    public class DriverSupportBL : IDriverSupportBL
    {
        private readonly IDriverSupportDB _driverSupportDB;

        private static readonly HashSet<string> ValidCategories = new(System.StringComparer.OrdinalIgnoreCase)
        { "Payment", "Trip", "Documents", "App", "Other" };

        public DriverSupportBL(IDriverSupportDB driverSupportDB)
        {
            _driverSupportDB = driverSupportDB;
        }

        public async Task<DriverSupport> GetSupportPageAsync(int driverId)
        {
            var tickets = await _driverSupportDB.GetTicketsAsync(driverId);

            return new DriverSupport
            {
                Faqs = GetFaqs(),
                Tickets = tickets.Select(t => new SupportTicket
                {
                    TicketId = t.TicketId,
                    Category = t.Category,
                    Subject = t.Subject,
                    Message = t.Message,
                    Status = t.Status,
                    AdminReply = t.AdminReply,
                    CreatedAt = t.CreatedAt
                }).ToList()
            };
        }

        public async Task<StatusUpdateResult> SubmitTicketAsync(int driverId, string category, string subject, string message)
        {
            if (string.IsNullOrWhiteSpace(category) || !ValidCategories.Contains(category.Trim()))
                return new StatusUpdateResult { Success = false, Message = "Please select a valid category." };

            if (string.IsNullOrWhiteSpace(subject))
                return new StatusUpdateResult { Success = false, Message = "Subject is required." };

            if (string.IsNullOrWhiteSpace(message) || message.Trim().Length < 10)
                return new StatusUpdateResult { Success = false, Message = "Please describe your issue in a bit more detail." };

            var id = await _driverSupportDB.CreateTicketAsync(driverId, category.Trim(), subject.Trim(), message.Trim());

            return id > 0
                ? new StatusUpdateResult { Success = true, Message = "Ticket submitted." }
                : new StatusUpdateResult { Success = false, Message = "Could not submit ticket. Please try again." };
        }

        private static List<FaqItem> GetFaqs() => new()
    {
        new FaqItem { Question = "When do I get paid for completed trips?", Answer = "Cash trips are collected directly from the passenger. Online payments are settled to your linked bank account within 24–48 hours of trip completion." },
        new FaqItem { Question = "Why can't I go online?", Answer = "You can only go online once your account shows 'Approved' status on your Documents page. If it's still pending, your documents are under review." },
        new FaqItem { Question = "What if a passenger cancels after I've started driving?", Answer = "You may be eligible for a cancellation fee depending on how far you'd travelled. Raise a ticket with the booking number and we'll review it." },
        new FaqItem { Question = "How do I update my vehicle details?", Answer = "Go to Profile → Documents, and re-upload your RC to update vehicle details. This will trigger a quick re-verification." },
        new FaqItem { Question = "My OTP isn't working to start a trip. What do I do?", Answer = "Confirm the OTP is being read from the passenger's booking confirmation, not a general app code. If it still fails, contact support with the booking number." }
    };
    }
}
