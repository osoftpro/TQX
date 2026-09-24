using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TnaboExtApp.Models
{
    public class HouseImage
    {
        [Key]
        public int Id { get; set; }
        public int? HouseId { get; set; }
        public string? HouseNo { get; set; }
        public byte[]? Image { get; set; }
        public string? RecordedBy { get; set; }

        [Column(TypeName ="datetime2")]
        public DateTime DateRecorded { get; set; }
    }
}
