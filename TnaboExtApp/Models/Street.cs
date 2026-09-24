using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TnaboExtApp.Models
{
    public class Street
    {
        [Key]
        public int Id { get; set; }
        public int PhaseId { get; set; }

        [Display(Name = "Phase Name")]
        public string? PhaseName { get; set; }

        [Display(Name = "Street Name")]
        public string? Name { get; set; }
        public string? StreetCode { get; set; }
        public string? CreatedBy { get; set; }

        public int HouseNumber { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime DateRecorded { get; set; }
    }
}
