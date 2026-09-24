using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TnaboExtApp.Models
{
    public class MonthlyIncome
    {
        [Key]
        public int Id { get; set; }

        [StringLength(16)]
        [Required]
        public string Period { get; set; } // Format: "2026-03"

        [Column(TypeName = "decimal(18,2)")]
        [DataType(DataType.Currency)]
        public decimal TotalExpected { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [DataType(DataType.Currency)]
        public decimal TotalPaid { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [DataType(DataType.Currency)]
        public decimal TotalBalance { get; set; }

        public int PaymentCount { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime DateRecorded { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime DateUpdated { get; set; }
    }
}