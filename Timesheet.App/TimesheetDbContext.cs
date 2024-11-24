using Microsoft.EntityFrameworkCore;
using Timesheet.App.Models;

namespace Timesheet.App
{
    public class TimesheetDbContext : DbContext
    {
        public TimesheetDbContext()
        {
        }


        public TimesheetDbContext(DbContextOptions<TimesheetDbContext> options) : base(options)
        {
        }


        public virtual DbSet<TimesheetEntry> Entries { get; set; }
    }
}
