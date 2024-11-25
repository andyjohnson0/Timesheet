using Microsoft.EntityFrameworkCore;
using Timesheet.App.Models;

namespace Timesheet.App
{
    /// <summary>
    /// Timesheet database context
    /// </summary>
    public class TimesheetDbContext : DbContext
    {
        /// <summary>
        /// Constructor. Initailise a TimesheetDbContext object.
        /// </summary>
        public TimesheetDbContext()
        {
        }


        /// <summary>
        /// Constructor. Initailise a TimesheetDbContext object.
        /// </summary>
        /// <param name="options">Database context options</param>
        public TimesheetDbContext(DbContextOptions<TimesheetDbContext> options) : base(options)
        {
        }


        /// <summary>
        /// DBSet containing the timesheet entries
        /// </summary>
        public virtual DbSet<TimesheetEntry> Entries { get; set; }


        /// <summary>
        /// Delete all entries in the DBSet
        /// </summary>
        public virtual void DeleteAll()
        {
            Entries.RemoveRange(this.Entries);
            SaveChanges();
        }
    }
}
