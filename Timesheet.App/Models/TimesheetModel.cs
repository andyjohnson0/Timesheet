namespace Timesheet.App.Models
{
    /// <summary>
    /// Timesheet model.
    /// Implements operations for adding a timesheet entry to the database and generating a timesheet as CSV text
    /// </summary>
    public class TimesheetModel
    {
        /// <summary>
        /// Constructor. Initialise a TimesheetModel object.
        /// </summary>
        /// <param name="db">Database context</param>
        public TimesheetModel(TimesheetDbContext db)
        {
            _db = db;
        }

        private readonly TimesheetDbContext _db;


        /// <summary>
        /// Add a timesheet entry to the database
        /// </summary>
        /// <param name="entry">Timesheet entry</param>
        /// <returns>True if the entry was saved, otherwise false</returns>
        public bool AddEntry(TimesheetEntry entry)
        {
            _db.Entries.Add(entry);
            return _db.SaveChanges() == 1;
        }
    }
}
