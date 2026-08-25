using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SardarJiCab.Model
{
    public class DriverDocuments
    {
        public int DriverId { get; set; }
        public string ApprovalStatus { get; set; } // Pending, Approved, Rejected, Suspended
        public string RejectionReason { get; set; }

        public string LicenseNumber { get; set; }
        public System.DateTime? LicenseExpiryDate { get; set; }
        public string LicensePhotoUrl { get; set; }

        public string AadhaarNumber { get; set; }
        public string AadhaarPhotoUrl { get; set; }

        public string RcNumber { get; set; }
        public string RcPhotoUrl { get; set; }

        public string AadhaarNumberMasked =>
            string.IsNullOrEmpty(AadhaarNumber) || AadhaarNumber.Length < 4
                ? AadhaarNumber
                : new string('X', AadhaarNumber.Length - 4) + AadhaarNumber[^4..];

        public string OverallStatusCss => (ApprovalStatus ?? "").ToLowerInvariant() switch
        {
            "approved" => "approved",
            "pending" => "pending",
            _ => "rejected" // Rejected or Suspended
        };

        public string OverallStatusTitle => (ApprovalStatus ?? "").ToLowerInvariant() switch
        {
            "approved" => "You're fully verified",
            "pending" => "Verification pending",
            "suspended" => "Account suspended",
            _ => "Action needed"
        };

        public string OverallStatusMessage => (ApprovalStatus ?? "").ToLowerInvariant() switch
        {
            "approved" => "All your documents have been reviewed and approved.",
            "pending" => "We're reviewing your uploaded documents. This usually takes 24–48 hours.",
            "suspended" => "Contact support to resolve your account status.",
            _ => "Please upload or correct the documents flagged below."
        };

        public bool HasLicense => !string.IsNullOrEmpty(LicensePhotoUrl);
        public bool HasAadhaar => !string.IsNullOrEmpty(AadhaarPhotoUrl);
        public bool HasRc => !string.IsNullOrEmpty(RcPhotoUrl);

        public int TotalDocumentsRequired => 3;
        public int DocumentsUploadedCount =>
            (HasLicense ? 1 : 0) + (HasAadhaar ? 1 : 0) + (HasRc ? 1 : 0);

        public int CompletionPercent => (int)(DocumentsUploadedCount / (double)TotalDocumentsRequired * 100);

        public string LicenseStatusCss => LicenseExpiryStatusCss(HasLicense, LicenseExpiryDate);
        public string LicenseStatusLabel => LicenseExpiryStatusLabel(HasLicense, LicenseExpiryDate);
        public string AadhaarStatusCss => HasAadhaar ? "uploaded" : "missing";
        public string AadhaarStatusLabel => HasAadhaar ? "Uploaded" : "Missing";
        public string RcStatusCss => HasRc ? "uploaded" : "missing";
        public string RcStatusLabel => HasRc ? "Uploaded" : "Missing";

        private static string LicenseExpiryStatusCss(bool hasFile, System.DateTime? expiry)
        {
            if (!hasFile) return "missing";
            if (expiry.HasValue && expiry.Value <= System.DateTime.UtcNow.AddDays(30)) return "expiring";
            return "uploaded";
        }

        private static string LicenseExpiryStatusLabel(bool hasFile, System.DateTime? expiry)
        {
            if (!hasFile) return "Missing";
            if (expiry.HasValue && expiry.Value <= System.DateTime.UtcNow) return "Expired";
            if (expiry.HasValue && expiry.Value <= System.DateTime.UtcNow.AddDays(30)) return "Expiring soon";
            return "Uploaded";
        }
    }
}
