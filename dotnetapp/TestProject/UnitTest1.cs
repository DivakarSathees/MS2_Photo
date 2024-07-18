using dotnetapp.Controllers;
using dotnetapp.Models;
using dotnetapp.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Reflection;
using System.Linq;

namespace dotnetapp.Tests
{
    [TestFixture]
    public class BookingControllerTests
    {
        private ApplicationDbContext _context;
        private BookingController _controller;
        private WorkshopController _workshopcontroller;

        [SetUp]
        public void Setup()
        {
            // Set up the test database context
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDbContext(options);
            _context.Database.EnsureCreated();

            _controller = new BookingController(_context);
            _workshopcontroller = new WorkshopController(_context);
        }

        [TearDown]
        public void TearDown()
        {
            // Clean up the test database context
            // _context.Database.EnsureDeleted();
            // _context.Dispose();
        }

        // This test checks if a valid workshop ID returns the WorkshopEnrollmentForm view
        [Test]
        public void WorkshopEnrollmentForm_ValidWorkshopId_ReturnsView()
        {
            // Arrange
            var workshop = new Workshop { Date = DateTime.Now, Title = "Demo title", Capacity = 10 };

            _context.Workshops.Add(workshop);
            _context.SaveChanges();

            // Act
            var result = _controller.WorkshopEnrollmentForm(workshop.WorkshopID) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(workshop, result.Model);
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        // This test checks if an invalid workshop ID returns NotFound result
        [Test]
        public void WorkshopEnrollmentForm_InvalidWorkshopId_ReturnsNotFound()
        {
            // Arrange
            var workshop = new Workshop { Date = DateTime.Now, Title = "Demo title", Capacity = 10 };
            _context.Workshops.Add(workshop);
            _context.SaveChanges();

            // Act
            var result = _controller.WorkshopEnrollmentForm(workshop.WorkshopID + 1);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        // This test checks if valid data creates a Participant and redirects to EnrollmentConfirmation action
        [Test]
        public void WorkshopEnrollmentForm_ValidData_CreatesParticipantAndRedirects()
        {
            // Arrange
            var workshop = new Workshop { Date = DateTime.Now, Title = "Demo title", Capacity = 3 };
            _context.Workshops.Add(workshop);
            _context.SaveChanges();

            // Act
            var result = _controller.WorkshopEnrollmentForm(workshop.WorkshopID, "John Doe", "john@example.com") as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("EnrollmentConfirmation", result.ActionName);

            // Check if the Participants was created and added to the database
            var Participants = _context.Participants.SingleOrDefault(s => s.WorkshopID == workshop.WorkshopID);
            Assert.IsNotNull(Participants);
            Assert.AreEqual("John Doe", Participants.Name);
            Assert.AreEqual("john@example.com", Participants.Email);
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        // This test checks if attempting to enroll in a full workshop throws a WorkshopBookingException
        [Test]
        public void WorkshopEnrollmentForm_WorkshopFull_ThrowsException()
        {
            // Arrange
            var workshop = new Workshop { Date = DateTime.Now, Title = "Demo title", Capacity = 0 };
            _context.Workshops.Add(workshop);
            _context.SaveChanges();

            // Assert
            Assert.Throws<WorkshopBookingException>(() =>
            {
                // Act
                _controller.WorkshopEnrollmentForm(workshop.WorkshopID, "John Doe", "john@example.com");
            });
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        // This test checks if attempting to enroll in a full workshop throws a WorkshopBookingException with the correct message
        [Test]
        public void WorkshopEnrollmentForm_WorkshopFull_ThrowsExceptionWithMessage()
        {
            // Arrange
            var workshop = new Workshop { Date = DateTime.Now, Title = "Demo title", Capacity = 0 };
            _context.Workshops.Add(workshop);
            _context.SaveChanges();

            var exception = Assert.Throws<WorkshopBookingException>(() =>
            {
                // Act
                _controller.WorkshopEnrollmentForm(workshop.WorkshopID, "John Doe", "john@example.com");
            });

            // Assert
            Assert.AreEqual("Maximum Number Reached", exception.Message);
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        // This test checks if attempting to confirm enrollment with a non-existent Participant ID returns NotFound result
        [Test]
        public void EnrollmentConfirmation_NonexistentParticipantId_ReturnsNotFound()
        {
            // Arrange
            var ParticipantID = 1;

            // Act
            var result = _controller.EnrollmentConfirmation(ParticipantID) as NotFoundResult;

            // Assert
            Assert.IsNotNull(result);
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        // This test checks if the Participant class exists and can be instantiated
        [Test]
        public void ParticipantClassExists()
        {
            var Participant = new Participant();

            Assert.IsNotNull(Participant);
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        // This test checks if the Workshop class exists and can be instantiated
        [Test]
        public void WorkshopClassExists()
        {
            var workshop = new Workshop();

            Assert.IsNotNull(workshop);
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        // This test checks if the ApplicationDbContext class contains a DbSet property for Workshops
        [Test]
        public void ApplicationDbContextContainsDbSetWorkshopProperty()
        {
            var propertyInfo = _context.GetType().GetProperty("Workshops");

            Assert.IsNotNull(propertyInfo);
            Assert.AreEqual(typeof(DbSet<Workshop>), propertyInfo.PropertyType);
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        // This test checks if the ApplicationDbContext class contains a DbSet property for Participant
        [Test]
        public void ApplicationDbContextContainsDbSetParticipantProperty()
        {
            var propertyInfo = _context.GetType().GetProperty("Participants");

            Assert.IsNotNull(propertyInfo);
            Assert.AreEqual(typeof(DbSet<Participant>), propertyInfo.PropertyType);
        }

        
        // This test checks the expected value of Date property in Workshop class
        [Test]
        public void Workshop_Properties_Date_ReturnExpectedValues()
        {
            DateTime expectedDate = new DateTime(2023, 7, 1, 9, 0, 0);

            Workshop workshop = new Workshop
            {
                Date = expectedDate
            };
            Assert.AreEqual(expectedDate, workshop.Date);
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        
        [Test]
        public void Participant_Properties_Name_ReturnExpectedDataTypes_String()
        {
            // Arrange
            string assemblyName = "dotnetapp";
            string typeName = "dotnetapp.Models.Participant";
            Assembly assembly = Assembly.Load(assemblyName);
            Type participantType = assembly.GetType(typeName);
            PropertyInfo propertyInfo = participantType.GetProperty("Name");
            
            // Assert
            Assert.IsNotNull(propertyInfo, "The property 'Name' was not found on the Participant class.");
            Type propertyType = propertyInfo.PropertyType;
            Assert.AreEqual(typeof(string), propertyType, "The data type of 'Name' property is not as expected (string).");
        }

        [Test]
        public void Participant_Properties_Email_ReturnExpectedDataTypes_String()
        {
            // Arrange
            string assemblyName = "dotnetapp";
            string typeName = "dotnetapp.Models.Participant";
            Assembly assembly = Assembly.Load(assemblyName);
            Type participantType = assembly.GetType(typeName);
            PropertyInfo propertyInfo = participantType.GetProperty("Email");
            
            // Assert
            Assert.IsNotNull(propertyInfo, "The property 'Email' was not found on the Participant class.");
            Type propertyType = propertyInfo.PropertyType;
            Assert.AreEqual(typeof(string), propertyType, "The data type of 'Email' property is not as expected (string).");
        }

        [Test]
        public void Participant_Properties_WorkshopID_ReturnExpectedDataTypes_Int()
        {
            // Arrange
            string assemblyName = "dotnetapp";
            string typeName = "dotnetapp.Models.Participant";
            Assembly assembly = Assembly.Load(assemblyName);
            Type participantType = assembly.GetType(typeName);
            PropertyInfo propertyInfo = participantType.GetProperty("WorkshopID");
            
            // Assert
            Assert.IsNotNull(propertyInfo, "The property 'WorkshopID' was not found on the Participant class.");
            Type propertyType = propertyInfo.PropertyType;
            Assert.AreEqual(typeof(int), propertyType, "The data type of 'WorkshopID' property is not as expected (int).");
        }

        [Test]
        public void Participant_Properties_Workshop_ReturnExpectedDataTypes_Workshop()
        {
            // Arrange
            string assemblyName = "dotnetapp";
            string typeName = "dotnetapp.Models.Participant";
            Assembly assembly = Assembly.Load(assemblyName);
            Type participantType = assembly.GetType(typeName);
            PropertyInfo propertyInfo = participantType.GetProperty("Workshop");
            
            // Assert
            Assert.IsNotNull(propertyInfo, "The property 'Workshop' was not found on the Participant class.");
            Type propertyType = propertyInfo.PropertyType;
            Assert.AreEqual(typeof(Workshop), propertyType, "The data type of 'Workshop' property is not as expected (Workshop).");
        }

        [Test]
        public void Workshop_Properties_Title_ReturnExpectedDataTypes_String()
        {
            // Arrange
            string assemblyName = "dotnetapp";
            string typeName = "dotnetapp.Models.Workshop";
            Assembly assembly = Assembly.Load(assemblyName);
            Type workshopType = assembly.GetType(typeName);
            PropertyInfo propertyInfo = workshopType.GetProperty("Title");
            
            // Assert
            Assert.IsNotNull(propertyInfo, "The property 'Title' was not found on the Workshop class.");
            Type propertyType = propertyInfo.PropertyType;
            Assert.AreEqual(typeof(string), propertyType, "The data type of 'Title' property is not as expected (string).");
        }

        [Test]
        public void Workshop_Properties_Date_ReturnExpectedDataTypes_DateTime()
        {
            // Arrange
            string assemblyName = "dotnetapp";
            string typeName = "dotnetapp.Models.Workshop";
            Assembly assembly = Assembly.Load(assemblyName);
            Type workshopType = assembly.GetType(typeName);
            PropertyInfo propertyInfo = workshopType.GetProperty("Date");
            
            // Assert
            Assert.IsNotNull(propertyInfo, "The property 'Date' was not found on the Workshop class.");
            Type propertyType = propertyInfo.PropertyType;
            Assert.AreEqual(typeof(DateTime), propertyType, "The data type of 'Date' property is not as expected (DateTime).");
        }

        [Test]
        public void Workshop_Properties_Capacity_ReturnExpectedDataTypes_Int()
        {
            // Arrange
            string assemblyName = "dotnetapp";
            string typeName = "dotnetapp.Models.Workshop";
            Assembly assembly = Assembly.Load(assemblyName);
            Type workshopType = assembly.GetType(typeName);
            PropertyInfo propertyInfo = workshopType.GetProperty("Capacity");
            
            // Assert
            Assert.IsNotNull(propertyInfo, "The property 'Capacity' was not found on the Workshop class.");
            Type propertyType = propertyInfo.PropertyType;
            Assert.AreEqual(typeof(int), propertyType, "The data type of 'Capacity' property is not as expected (int).");
        _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public void Workshop_Properties_Participants_ReturnExpectedDataTypes_ICollection()
        {
            // Arrange
            string assemblyName = "dotnetapp";
            string typeName = "dotnetapp.Models.Workshop";
            Assembly assembly = Assembly.Load(assemblyName);
            Type workshopType = assembly.GetType(typeName);
            PropertyInfo propertyInfo = workshopType.GetProperty("Participants");
            
            // Assert
            Assert.IsNotNull(propertyInfo, "The property 'Participants' was not found on the Workshop class.");
            Type propertyType = propertyInfo.PropertyType;
            Assert.AreEqual(typeof(ICollection<Participant>), propertyType, "The data type of 'Participants' property is not as expected (ICollection<Participant>).");
       _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task AvailableWorkshops_ReturnsViewWithAllWorkshops()
        {
            // Arrange
            var workshops = new List<Workshop>
            {
                new Workshop { Date = DateTime.Now, Title = "Workshop 1", Capacity = 10 },
                new Workshop { Date = DateTime.Now.AddDays(1), Title = "Workshop 2", Capacity = 15 }
            };

            _context.Workshops.AddRange(workshops);
            _context.SaveChanges();

            // Act
            var result = await _workshopcontroller.AvailableWorkshops() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            var model = result.Model as List<Workshop>;
            Assert.IsNotNull(model);
            Assert.AreEqual(4, model.Count);
            // Assert.AreEqual("Workshop 1", model[0].Title);
            // Assert.AreEqual("Workshop 2", model[1].Title);
        }

        // This test checks if BookedWorkshops method returns the correct view with only booked workshops
        [Test]
        public async Task BookedWorkshops_ReturnsViewWithBookedWorkshops()
        {
            
            // Arrange
            var workshop1 = new Workshop { Date = DateTime.Now, Title = "Workshop 1", Capacity = 10 };
            var workshop2 = new Workshop { Date = DateTime.Now.AddDays(1), Title = "Workshop 2", Capacity = 15 };
            var participant = new Participant { Name = "John Doe", Email = "john@example.com", WorkshopID = 3 };

            _context.Workshops.AddRange(workshop1, workshop2);
            _context.Participants.Add(participant);
            _context.SaveChanges();

            // Act
            var result = await _workshopcontroller.BookedWorkshops() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            var model = result.Model as List<Workshop>;
            Assert.IsNotNull(model);
            Assert.AreEqual(1, model.Count);
            Assert.AreEqual("Workshop 1", model[0].Title);
        }

        // This test checks if BookedWorkshops method returns the correct view with no workshops when there are no booked workshops
        [Test]
        public async Task BookedWorkshops_ReturnsViewWithNoWorkshops()
        {
            // Arrange
            var workshop1 = new Workshop { Date = DateTime.Now, Title = "Workshop 1", Capacity = 10 };
            var workshop2 = new Workshop { Date = DateTime.Now.AddDays(1), Title = "Workshop 2", Capacity = 15 };

            _context.Workshops.AddRange(workshop1, workshop2);
            _context.SaveChanges();

            // Act
            var result = await _workshopcontroller.BookedWorkshops() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            var model = result.Model as List<Workshop>;
            Assert.IsNotNull(model);
            Assert.AreEqual(0, model.Count);
        }
    }
}
