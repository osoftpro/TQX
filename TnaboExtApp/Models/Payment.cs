using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TnaboExtApp.Models
{
    public class Payment
    {
        [Key]
        public int Id { get; set; }

        // Optional relation to House
        public int? HouseId { get; set; }
        public string? HouseName { get; set; }
        public string? HouseNo { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [DataType(DataType.Currency)]
        [Required]
        public decimal Amount { get; set; }

        // Amount paid so far for this payment record (persisted)
        [Column(TypeName = "decimal(18,2)")]
        [DataType(DataType.Currency)]
        public decimal AmountPaid { get; set; } = 0m;

        // Calculated remaining balance (not stored)
        [NotMapped]
        [DataType(DataType.Currency)]
        public decimal BalanceLeft => Amount - AmountPaid;

        // Format like "2026-03" or "March 2026"
        [StringLength(16)]
        public string? Period { get; set; }

        [Column(TypeName = "datetime2")]
        [Display(Name = "Payment Date")]
        public DateTime PaymentDate { get; set; }

        [StringLength(64)]
        public string? Method { get; set; }

        // e.g. Paid, Partial, Pending
        [StringLength(32)]
        public string? Status { get; set; }

        public string? RecordedBy { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime DateRecorded { get; set; }

        // Navigation (optional)
        public House? House { get; set; }
    }
}