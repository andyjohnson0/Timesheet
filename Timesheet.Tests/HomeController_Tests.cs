using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Timesheet.App;
using Timesheet.App.Controllers;
using Timesheet.App.Models;


namespace Timesheet.Tests
{
    [TestClass]
    public class HomeController_Tests
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
            var result = controller.Add(_testEntries[0]) as OkResult;

            // Assert
            Assert.IsNotNull(result);  // Verify that Add() returned success
            Assert.AreEqual(1, data.Count);  // Verify the entry was added.
            Assert.AreEqual(_testEntries[0].UserName, data[0].UserName);  // Verify entry present in backing list
            mockDbContext.Verify(c => c.SaveChanges(), Times.Once);  // Verify SaveChanges was called.
        }


        [TestMethod]
        public void Timesheet_ValidCsv()
        {
            // Arrange
            var mockSet = new Mock<DbSet<TimesheetEntry>>();
            var data = new List<TimesheetEntry>();
            mockSet.Setup(m => m.Add(It.IsAny<TimesheetEntry>()));

            var mockDbContext = new Mock<TimesheetDbContext>();

            var controller = new HomeController(mockDbContext.Object, _mockLogger!.Object);

            // Act
            var result = controller.Timesheet() as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            var csvText = result.Value as string;
            Assert.IsNotNull(csvText);
            Assert.AreEqual("", csvText);
        }
    }
}
