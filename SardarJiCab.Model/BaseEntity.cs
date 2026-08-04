using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SardarJiCab.Model
{
    public class BaseEntity
    {
        [Key]
        public Int64 Id { get; set; }

        // ---------------- Timestamps ----------------

        public DateTime CreatedOn { get; set; } = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));

        public DateTime? UpdatedOn { get; set; } = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
        public Int64 CreatedBy { get; set; }
        public Int64 UpdatedBy { get; set; }

        // ---------------- Response ----------------
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}
