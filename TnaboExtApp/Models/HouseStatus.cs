using System.ComponentModel.DataAnnotations;

namespace TnaboExtApp.Models
{
    public class HouseStatus
    {
        [Key]
        public int? id { get; set; }
        public string? Name { get; set; }
    }
}
