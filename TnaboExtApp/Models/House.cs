using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace TnaboExtApp.Models
{
    public class House
    {

        [Key]
        public int Id { get; set; }
        public string? OwnerName { get; set; }

        public string? StreetName { get; set; }
        public int? StreetId { get; set; }

        public string? PhaseName { get; set; }
        public int? PhaseId { get; set; }

        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Longitude { get; set; }
        public string? Latitude { get; set; }
        public string? Status { get; set; }
        public string? HouseNo { get; set; }

        public byte[]? Photo { get; set; }
        public string? CreatedBy { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime DateRecorded { get; set; }
    }
}
