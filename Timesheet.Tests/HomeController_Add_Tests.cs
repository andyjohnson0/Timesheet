using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using Timesheet.App;
using Timesheet.App.Controllers;
using Timesheet.App.Models;


namespace Timesheet.Tests
{
    /// <summary>
    /// Tests for the Add endpoint
    /// </summary>
    [TestClass]
    public class HomeController_Add_Tests
    {
        [TestInitialize]
        public void SetUp()
        {
            _mockLogger = new Mock<ILogger<HomeController>>();
        }

        private Mock<ILogger<HomeController>>? _mockLogger;

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
        public void Add_ValidEntry_AddsToDatabase()
        {
            // Arrange
            var mockSet = new Mock<DbSet<TimesheetEntry>>();
            var data = new List<TimesheetEntry>();
            mockSet
                .Setup(m => m.Add(It.IsAny<TimesheetEntry>()))
                .Callback<TimesheetEntry>(entry => data.Add(entry));

            var mockDbContext = new Mock<TimesheetDbContext>();
            mockDbContext.Setup(c => c.Entries).Returns(mockSet.Object);
            mockDbContext.Setup(c => c.SaveChanges()).Returns(1);

            var controller = new HomeController(mockDbContext.Object, _mockLogger!.Object);

            // Act
            var result = controller.Add(_testEntries[0]);

            // Assert
            Assert.IsTrue(result is OkResult || result is RedirectToActionResult);
            Assert.AreEqual(1, data.Count);  // Verify the entry was added.
            Assert.AreEqual(_testEntries[0].UserName, data[0].UserName);  // Verify entry present in backing list
            mockDbContext.Verify(c => c.SaveChanges(), Times.Once);  // Verify SaveChanges was called.
        }


        [TestMethod]
        public void Add_InvalidEntry_UserNameEmpty_NotAddedToDatabase()
        {
            var invalidEntry = new TimesheetEntry
            {
                UserName = "",  // Invalid
                Date = _testEntries[0].Date,
                ProjectName = _testEntries[0].ProjectName,
                TaskDescription = _testEntries[0].TaskDescription,
                HoursWorked = _testEntries[0].HoursWorked,
            };
            DoAdd_InvalidEntry_NotAddedToDatabase(invalidEntry);
        }


        [TestMethod]
        public void Add_InvalidEntry_FutureDate_NotAddedToDatabase()
        {
            var invalidEntry = new TimesheetEntry
            {
                UserName = _testEntries[0].UserName,
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(7)),  // Invalid
                ProjectName = _testEntries[0].ProjectName,
                TaskDescription = _testEntries[0].TaskDescription,
                HoursWorked = _testEntries[0].HoursWorked,
            };
            DoAdd_InvalidEntry_NotAddedToDatabase(invalidEntry);
        }


        [TestMethod]
        public void Add_InvalidEntry_ProjectNameEmpty_NotAddedToDatabase()
        {
            var invalidEntry = new TimesheetEntry
            {
                UserName = _testEntries[0].UserName,
                Date = _testEntries[0].Date,
                ProjectName = "",  // Invalid
                TaskDescription = _testEntries[0].TaskDescription,
                HoursWorked = _testEntries[0].HoursWorked,
            };
            DoAdd_InvalidEntry_NotAddedToDatabase(invalidEntry);
        }


        [TestMethod]
        public void Add_InvalidEntry_InvalidHoursWorked_NotAddedToDatabase()
        {
            var invalidEntry = new TimesheetEntry
            {
                UserName = _testEntries[0].UserName,
                Date = _testEntries[0].Date,
                ProjectName = _testEntries[0].ProjectName,
                TaskDescription = _testEntries[0].TaskDescription,
                HoursWorked = 0,  // Invalid
            };
            DoAdd_InvalidEntry_NotAddedToDatabase(invalidEntry);
        }


        private void DoAdd_InvalidEntry_NotAddedToDatabase(TimesheetEntry invalidEntry)
        {
            // Arrange
            var mockSet = new Mock<DbSet<TimesheetEntry>>();
            var mockDbContext = new Mock<TimesheetDbContext>();
            mockDbContext.Setup(c => c.Entries).Returns(mockSet.Object);

            var controller = new HomeController(mockDbContext.Object, _mockLogger!.Object);

            // Act
            controller.DoAspNetModelValidation(invalidEntry);
            controller.Add(invalidEntry);

            // Assert
            mockSet.Verify(m => m.Add(It.IsAny<TimesheetEntry>()), Times.Never); // Ensure Add was not called.
            mockDbContext.Verify(c => c.SaveChanges(), Times.Never); // Ensure SaveChanges was not called.
        }
    }


    /// <summary>
    /// Testing-related extensions to HomeController class
    /// </summary>
    public static class HomeControllerExt
    {
        /// <summary>
        /// Do model validation on a timesheet entry.
        /// This function performs that validation that ASP.NET MVC does at run-time.
        /// </summary>
        /// <param name="entry">Timesheet entry to validate</param>
        public static void DoAspNetModelValidation(this HomeController self, TimesheetEntry entry)
        {
            // Validate the model and populate ModelState
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(entry);
            Validator.TryValidateObject(entry, validationContext, validationResults, true);
            foreach (var validationResult in validationResults)
            {
                self.ModelState.AddModelError(validationResult.MemberNames.FirstOrDefault() ?? string.Empty, validationResult.ErrorMessage!);
            }
        }
    }
}
