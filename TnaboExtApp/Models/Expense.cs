using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TnaboExtApp.Models
{
    public class Expense
    {
        [Key]
        public int Id { get; set; }

        [StringLength(64)]
        [Required]
        public string Category { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        [DataType(DataType.Currency)]
        [Required]
        public decimal Amount { get; set; }

        [Column(TypeName = "datetime2")]
        [Display(Name = "Expense Date")]
        public DateTime Date { get; set; }

        [StringLength(512)]
        public string? Description { get; set; }

        // Optional link to a house if expense is house-specific
        public int? RelatedHouseId { get; set; }

        // URL or blob id for a receipt (optional)
        [StringLength(256)]
        public string? ReceiptUrl { get; set; }

        public string? RecordedBy { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime DateRecorded { get; set; }

        public House? RelatedHouse { get; set; }
    }
}