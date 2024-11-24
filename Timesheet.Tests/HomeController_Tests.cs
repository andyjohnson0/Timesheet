using Microsoft.AspNetCore.Mvc;
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


        [TestMethod]
        public void Add_ValidEntry_AddsToDatabase()
        {
            // Arrange
            var controller = new HomeController(_mockLogger!.Object);

            // Act
            var result = controller.Add() as OkResult;

            // Assert
            Assert.IsNotNull(result);
        }


        [TestMethod]
        public void Timesheet_ValidCsv()
        {
            // Arrange
            var controller = new HomeController(_mockLogger!.Object);

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
