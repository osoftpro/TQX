using System.ComponentModel.DataAnnotations;

namespace TnaboExtApp.Models
{
    public class Phase
    {
        [Key]
        public int Id { get; set; }

        [Display(Name ="Phase Name")]
        public string? Name { get; set; }

        [Display(Name ="Code")]
        public string? PhaseCode { get; set; }
    }
}
