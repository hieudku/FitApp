using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using FitApp.Controllers;
using FitApp.Data;
using FitApp.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace FitApp.Tests
{
    [TestClass]
    public class UserSpecificWorkoutsControllerTests
    {
        private Mock<UserManager<IdentityUser>> _userManagerMock;
        private Mock<ILogger<UserSpecificWorkoutsController>> _loggerMock;
        private FitAppContext _context;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<FitAppContext>()
                .UseInMemoryDatabase(databaseName: "FitAppTest")
                .Options;
            _context = new FitAppContext(options);

            _userManagerMock = new Mock<UserManager<IdentityUser>>(
                Mock.Of<IUserStore<IdentityUser>>(), null, null, null, null, null, null, null, null);
            _loggerMock = new Mock<ILogger<UserSpecificWorkoutsController>>();

            // Clear the in-memory database before seeding
            _context.UserSpecificWorkouts.RemoveRange(_context.UserSpecificWorkouts);
            _context.SaveChanges();

            // Seed the in-memory database with test data
            _context.UserSpecificWorkouts.AddRange(
                new UserSpecificWorkout { Id = 1, UserId = "user1", Name = "Workout1", CaloriesBurned = 100, Duration = TimeSpan.FromMinutes(30) },
                new UserSpecificWorkout { Id = 2, UserId = "user1", Name = "Workout2", CaloriesBurned = 200, Duration = TimeSpan.FromMinutes(45) },
                new UserSpecificWorkout { Id = 3, UserId = "user2", Name = "Workout3", CaloriesBurned = 150, Duration = TimeSpan.FromMinutes(40) }
            );
            _context.SaveChanges();
        }

        /// <summary>
        /// READ: Tests that the Index method returns MyWorkouts View and data for logged in user.
        /// By verifying that view name is MyWorkouts and model contains list of saved (seeded in testing) UserSpecificWorkouts.
        /// </summary>
        /// 
        [TestMethod]
        public async Task Index_ReturnsCorrectViewAndData()
        {
            // Arrange
            var controller = new UserSpecificWorkoutsController(_context, _userManagerMock.Object, _loggerMock.Object);
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "user1")
            }, "mock"));
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            _userManagerMock.Setup(um => um.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns("user1");

            // Act
            var result = await controller.Index(null, null) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("MyWorkouts", result.ViewName);
            var model = result.Model as List<UserSpecificWorkout>;
            Assert.IsNotNull(model);
            Assert.AreEqual(2, model.Count);
            Assert.AreEqual("Workout1", model[0].Name);
            Assert.AreEqual("Workout2", model[1].Name);
        }

        /// <summary>
        /// DELETE: Tests that the DeleteUserWorkout method returns NotFound when the provided ID is null (user ID not found or missing).
        /// </summary>
        /// 
        [TestMethod]
        public async Task DeleteUserWorkout_ReturnsNotFound_WhenIdIsNull()
        {
            // Arrange
            var controller = new UserSpecificWorkoutsController(_context, _userManagerMock.Object, _loggerMock.Object);

            // Act
            var result = await controller.DeleteUserWorkout(null);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        /// <summary>
        /// DELETE: Tests that the DeleteUserWorkout method returns NotFound when the workout with the provided ID is not found.
        /// </summary>
        /// 
        [TestMethod]
        public async Task DeleteUserWorkout_ReturnsNotFound_WhenWorkoutNotFound()
        {
            // Arrange
            var controller = new UserSpecificWorkoutsController(_context, _userManagerMock.Object, _loggerMock.Object);
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
        new Claim(ClaimTypes.NameIdentifier, "user1")
            }, "mock"));
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            // Act
            var result = await controller.DeleteUserWorkout(999);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        /// <summary>
        /// READ: Tests that the Details method returns NotFound when the provided ID is null.
        /// Where assume the ID parameter is missing (null).
        /// </summary>
        /// 
        [TestMethod]
        public async Task Details_ReturnsNotFound_WhenIdIsNull()
        {
            // Arrange
            var controller = new UserSpecificWorkoutsController(_context, _userManagerMock.Object, _loggerMock.Object);

            // Act
            var result = await controller.Details(null);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        /// <summary>
        /// Tests that the Details method returns NotFound when the workout with the provided ID is not found.
        /// This makes sure that the method correctly handles cases where the specified workout does not exist in the database.
        /// </summary>
        [TestMethod]
        public async Task Details_ReturnsNotFound_WhenWorkoutNotFound()
        {
            // Arrange
            var controller = new UserSpecificWorkoutsController(_context, _userManagerMock.Object, _loggerMock.Object);

            // Act
            var result = await controller.Details(999);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        /// <summary>
        /// Tests that the Details method returns a ViewResult (Details view) with the correct UserSpecificWorkout when a valid ID is provided.
        /// To verifies that the method retrieves and displays the correct workout details for the logged-in user
        /// 
        [TestMethod]
        public async Task Details_ReturnsViewResult_WithUserWorkout()
        {
            // Arrange
            var controller = new UserSpecificWorkoutsController(_context, _userManagerMock.Object, _loggerMock.Object);

            // Act
            var result = await controller.Details(1) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            var model = result.Model as UserSpecificWorkout;
            Assert.IsNotNull(model);
            Assert.AreEqual(1, model.Id);
        }

        /// <summary>
        /// FILTER: Tests that the Index method correctly filters workouts based on the search string
        /// This ensures that only workouts matching the search term are returned.
        /// </summary>
        /// 
        [TestMethod]
        public async Task Index_FiltersWorkouts_BySearchString()
        {
            // Arrange
            var controller = new UserSpecificWorkoutsController(_context, _userManagerMock.Object, _loggerMock.Object);
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
        new Claim(ClaimTypes.NameIdentifier, "user1")
            }, "mock"));
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            _userManagerMock.Setup(um => um.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns("user1");

            // Act
            var result = await controller.Index(null, "Workout1") as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("MyWorkouts", result.ViewName);
            var model = result.Model as List<UserSpecificWorkout>;
            Assert.IsNotNull(model);
            Assert.AreEqual(1, model.Count);
            Assert.AreEqual("Workout1", model[0].Name);
        }


        /// <summary>
        /// Tests that the Index method correctly sorts workouts by name in descending order
        /// that workouts are sorted by name in the correct order.
        /// </summary>
        [TestMethod]
        public async Task Index_SortsWorkouts_ByNameDescending()
        {
            // Arrange
            var controller = new UserSpecificWorkoutsController(_context, _userManagerMock.Object, _loggerMock.Object);
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
        new Claim(ClaimTypes.NameIdentifier, "user1")
            }, "mock"));
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            _userManagerMock.Setup(um => um.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns("user1");

            // Act
            var result = await controller.Index("name_desc", null) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("MyWorkouts", result.ViewName);
            var model = result.Model as List<UserSpecificWorkout>;
            Assert.IsNotNull(model);
            Assert.AreEqual(2, model.Count);
            Assert.AreEqual("Workout2", model[0].Name); // Workout2 is on top of Workout1 in descending order
            Assert.AreEqual("Workout1", model[1].Name);
        }

        /// <summary>
        /// Tests that the Index method correctly sorts workouts by calories burned in ascending order
        /// that workouts are sorted by calories burned in the correct order.
        /// </summary>
        [TestMethod]
        public async Task Index_SortsWorkouts_ByCaloriesAscending()
        {
            // Arrange
            var controller = new UserSpecificWorkoutsController(_context, _userManagerMock.Object, _loggerMock.Object);
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
        new Claim(ClaimTypes.NameIdentifier, "user1")
            }, "mock"));
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            _userManagerMock.Setup(um => um.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns("user1");

            // Act
            var result = await controller.Index("Calories", null) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("MyWorkouts", result.ViewName);
            var model = result.Model as List<UserSpecificWorkout>;
            Assert.IsNotNull(model);
            Assert.AreEqual(2, model.Count);
            Assert.AreEqual(100, model[0].CaloriesBurned); // 100 is on top of 200 in ascending order
            Assert.AreEqual(200, model[1].CaloriesBurned);
        }

        /// <summary>
        /// SORT: Tests that the Index method correctly sorts workouts by duration in descending order
        /// that workouts are sorted by duration in the correct order.
        /// </summary>
        [TestMethod]
        public async Task Index_SortsWorkouts_ByDurationDescending()
        {
            // Arrange
            var controller = new UserSpecificWorkoutsController(_context, _userManagerMock.Object, _loggerMock.Object);
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
        new Claim(ClaimTypes.NameIdentifier, "user1")
            }, "mock"));
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            _userManagerMock.Setup(um => um.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns("user1");

            // Act
            var result = await controller.Index("duration_desc", null) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("MyWorkouts", result.ViewName);
            var model = result.Model as List<UserSpecificWorkout>;
            Assert.IsNotNull(model);
            Assert.AreEqual(2, model.Count);
            Assert.AreEqual(TimeSpan.FromMinutes(45), model[0].Duration); // 45 mins is on top of 30 mins in descending order
            Assert.AreEqual(TimeSpan.FromMinutes(30), model[1].Duration);
        }

    }
}
