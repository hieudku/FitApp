using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using FitApp.Controllers;
using FitApp.Data;
using FitApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace FitApp.Tests
{
    /// <summary>
    /// This test class uses mock objects to simulate the behavior of the UserSpecificWorkoutsController in a controlled environment.
    /// By using Moq, we mock the DbSet and DbContext (FitAppContext) to avoid hitting a real database. 
    /// This allows us to test the controller's behavior, such as filtering, sorting, viewing, and deleting user-specific workouts, without requiring actual data access.
    ///
    /// Key functionalities tested include:
    /// 1. **Read Operation (Index and MyWorkouts)**: Verifying the controller retrieves and displays user-specific workouts correctly, including search and sorting logic.
    /// 2. **Delete Operation**: Testing if user-specific workouts are fetched correctly for deletion and are deleted as expected.
    /// 3. **Save Operation**: Testing if a workout from the master list can be saved to the user-specific workout list.
    /// 4. **Details Operation**: Verifying that a user can view detailed information about their own workouts.
    /// 
    /// By mocking dependencies such as UserManager, DbContext, and Logger, the tests focus on controller behavior independently of actual database interactions.
    /// </summary>
    [TestClass]
    public class UserSpecificWorkoutsControllerTests
    {
        private FitAppContext _context;
        private Mock<UserManager<IdentityUser>> _mockUserManager;
        private Mock<ILogger<UserSpecificWorkoutsController>> _mockLogger;
        private UserSpecificWorkoutsController _controller;

        [TestInitialize]
        public void Setup()
        {
            // Set up in-memory database options
            var options = new DbContextOptionsBuilder<FitAppContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            // Create a new FitAppContext using the in-memory database
            _context = new FitAppContext(options);

            _mockUserManager = new Mock<UserManager<IdentityUser>>(
                Mock.Of<IUserStore<IdentityUser>>(), null, null, null, null, null, null, null, null);
            _mockLogger = new Mock<ILogger<UserSpecificWorkoutsController>>();

            // Set up a mocked user for testing
            var user = new IdentityUser { Id = "user1", UserName = "testuser" };
            var userClaims = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
        new Claim(ClaimTypes.NameIdentifier, user.Id)
            }));

            var mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.Setup(c => c.User).Returns(userClaims);

            var controllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext.Object
            };

            _controller = new UserSpecificWorkoutsController(_context, _mockUserManager.Object, _mockLogger.Object)
            {
                ControllerContext = controllerContext
            };

            // Seed data for testing
            _context.UserSpecificWorkouts.Add(new UserSpecificWorkout { Id = 1, Name = "Yoga", Description = "Relaxing workout", UserId = "user1" });
            _context.UserSpecificWorkouts.Add(new UserSpecificWorkout { Id = 2, Name = "HIIT", Description = "High intensity", UserId = "user1" });
            _context.SaveChanges();
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        // Test the Index action for listing workouts (with search and sorting)
        



        // Test Details (GET)
        [TestMethod]
        public async Task Details_Get_WorkoutExists_ReturnsViewWithWorkout()
        {
            // Act - Call the Details method
            var result = await _controller.Details(1) as ViewResult;
            var model = result.Model as UserSpecificWorkout;

            // Assert
            Assert.IsNotNull(model);
            Assert.AreEqual("Yoga", model.Name); // Verify that the workout is "Yoga"
        }
    }
}