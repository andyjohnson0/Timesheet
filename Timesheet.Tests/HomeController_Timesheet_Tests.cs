using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Timesheet.App;
using Timesheet.App.Controllers;
using Timesheet.App.Models;


namespace Timesheet.Tests
{
    [TestClass]
    public class HomeController_Timesheet_Tests
    {
        private Mock<ILogger<HomeController>>? _mockLogger;

        [TestInitialize]
        public void SetUp()
        {
            // Initialize mock DbContext and controller
            _mockLogger = new Mock<ILogger<HomeController>>();
        }


        private readonly TimesheetEntry[] _testEntries =
        [
            new TimesheetEntry()
            {
                Date = new DateOnly(2014, 10, 22),
                UserName = "John Smith",
                ProjectName = "Project Alpha",
                TaskDescription = "Developed new feature X",
                HoursWorked = 4
            },
            new TimesheetEntry()
            {
                Date = new DateOnly(2014, 10, 22),
                UserName = "John Smith",
                ProjectName = "Project Beta",
                TaskDescription = "Developed new feature Y",
                HoursWorked = 4
            },
            new TimesheetEntry()
            {
                Date = new DateOnly(2014, 10, 22),
                UserName = "Jane Doe",
                ProjectName = "Project Gamma",
                TaskDescription = "Conducted user testing",
                HoursWorked = 6
            }
        ];



        [TestMethod]
        public void Timesheet_MultipleEntries_ValidCsv()
        {
            // Arrange. Note that we are using a real in-memory database, not a mock
            var options = new DbContextOptionsBuilder<TimesheetDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            using var dbContext = new TimesheetDbContext(options);
            dbContext.DeleteAll();
            dbContext.Entries.AddRange(_testEntries);  // Add test data
            dbContext.SaveChanges();

            var controller = new HomeController(dbContext, _mockLogger!.Object);

            // Act
            var result = controller.Timesheet() as OkObjectResult;

            // Assert
            const int expectedLineCount = 4;  // 3 data lines, plus 1 header line
            const int expectedElementCount = 6;
            var expectedWorkingHours = new int[]
            {
                _testEntries[0].HoursWorked + _testEntries[1].HoursWorked,
                _testEntries[0].HoursWorked + _testEntries[1].HoursWorked,
                _testEntries[2].HoursWorked
            };
            Assert.IsNotNull(result);
            var csvText = result.Value as string;
            Assert.IsNotNull(csvText);
            Assert.AreNotEqual("", csvText);
            var csvLines = csvText.TrimEnd().Split(Environment.NewLine);  // remove trailing blank line
            Assert.AreEqual(expectedLineCount, csvLines.Length);
            for (int iLine = 0; iLine < expectedLineCount; iLine++)
            {
                // Extract elements from lines
                var elems = csvLines[iLine].Split(',');
                // Header and data lines
                Assert.AreEqual(expectedElementCount, elems.Length);
                if (iLine >= 1)
                {
                    // Data lines only
                    Assert.AreEqual(_testEntries[iLine - 1].UserName, elems[0]);
                    Assert.AreEqual(_testEntries[iLine - 1].Date.ToString("d"), elems[1]);
                    Assert.AreEqual(_testEntries[iLine - 1].ProjectName, elems[2]);
                    Assert.AreEqual(_testEntries[iLine - 1].TaskDescription, elems[3]);
                    Assert.AreEqual(_testEntries[iLine - 1].HoursWorked.ToString(), elems[4]);
                    Assert.AreEqual(expectedWorkingHours[iLine - 1].ToString(), elems[5]);
                }
            }
        }


        [TestMethod]
        public void Timesheet_NoEntries_ValidCsv()
        {
            // Arrange. Note that we are using a real in-memory database, not a mock
            var options = new DbContextOptionsBuilder<TimesheetDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            using var dbContext = new TimesheetDbContext(options);
            dbContext.DeleteAll();  // Ensure the dbset is empty

            var controller = new HomeController(dbContext, _mockLogger!.Object);

            // Act
            var result = controller.Timesheet() as OkObjectResult;

            // Assert
            const int expectedLineCount = 1;  // header line only
            const int expectedElementCount = 6;
            Assert.IsNotNull(result);
            var csvText = result.Value as string;
            Assert.IsNotNull(csvText);
            Assert.AreNotEqual("", csvText);
            var csvLines = csvText.TrimEnd().Split(Environment.NewLine);  // remove trailing blank line
            Assert.AreEqual(expectedLineCount, csvLines.Length);
            var elems = csvLines[0].Split(',');
            Assert.AreEqual(expectedElementCount, elems.Length);
        }
    }
}
