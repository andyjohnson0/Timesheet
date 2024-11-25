using System.ComponentModel.DataAnnotations;

namespace Timesheet.App.Models
{
    /// <summary>
    /// Timesheet entry representation
    /// </summary>
    public class TimesheetEntry
    {
        public int Id { get; set; }

        [Required]
        public DateOnly Date { get; set; }

        [Required]
        [MinLength(3)]
        public string UserName { get; set; } = "";

        [Required]
        [MinLength(3)]
        public string ProjectName { get; set; } = "";

        public string? TaskDescription { get; set; }

        [Required]
        [Range(1, 24)]
        public int HoursWorked { get; set; } = 0;
    }
}
