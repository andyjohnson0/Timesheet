using System.Text;

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


        /// <summary>
        /// Generate a CSV text timesheet from the database contents
        /// </summary>
        /// <returns>Timesheet as CSV text</returns>
        public string GetTimesheetAsCSV()
        {
            var sb = new StringBuilder();
            sb.AppendLine("User Name,Date,Project,Description of Tasks,Hours Worked,Total Hours for the Day");

            // Query with grouping and summation to generate the total hours worked per day, per the problem statement, and build the CSV
            var query = _db.Entries
                .GroupBy(entry => new { entry.UserName, entry.Date })
                .Select(group => new
                {
                    UserName = group.Key.UserName,
                    Date = group.Key.Date,
                    TotalHoursWorked = group.Sum(entry => entry.HoursWorked),
                    Entries = group.Select(entry => new
                    {
                        entry.ProjectName,
                        entry.TaskDescription,
                        entry.HoursWorked
                    }).ToList()
                })
                .OrderBy(result => result.Date);
            foreach (var result in query)
            {
                foreach (var entry in result.Entries)
                {
                    var line = $"{CsvEscape(result.UserName)},{result.Date.ToString("d")},{CsvEscape(entry.ProjectName)},{CsvEscape(entry.TaskDescription!)},{entry.HoursWorked},{result.TotalHoursWorked}";
                    sb.AppendLine(line);
                }
            }

            return sb.ToString();
        }


        /// <summary>
        /// helper function to implement CSV escaping semantics for elements contains quotes for commas
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static string CsvEscape(string str)
        {
            var escape = str.Contains('"') || str.Contains(',');
            if (escape)
            {
                // " -> "", xxx,yyy -> "xxx,yyy"
                var escStr = str.Replace("\"", "\"\"");
                if (escStr.Contains(','))
                    escStr = '"' + escStr + '"';
                return escStr;
            }
            else
            {
                return str;
            }
        }
    }
}
