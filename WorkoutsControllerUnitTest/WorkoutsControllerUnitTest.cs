using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using FitApp.Controllers;
using FitApp.Data;
using FitApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace FitApp.Tests
{
    [TestClass]
    public class WorkoutsControllerTests
    {
        private Mock<FitAppContext> _mockContext;
        private WorkoutsController _controller;

        // Initialize mock context before each test
        [TestInitialize]
        public void Setup()
        {
            // Mock data for Workouts
            var workouts = new List<Workouts>
            {
                new Workouts { Id = 1, Name = "Workout A", Description = "Test A" },
                new Workouts { Id = 2, Name = "Workout B", Description = "Test B" }
            }.AsQueryable();

            // Mock the DbSet for Workouts
            var mockSet = new Mock<DbSet<Workouts>>();
            mockSet.As<IQueryable<Workouts>>().Setup(m => m.Provider).Returns(workouts.Provider);
            mockSet.As<IQueryable<Workouts>>().Setup(m => m.Expression).Returns(workouts.Expression);
            mockSet.As<IQueryable<Workouts>>().Setup(m => m.ElementType).Returns(workouts.ElementType);
            mockSet.As<IQueryable<Workouts>>().Setup(m => m.GetEnumerator()).Returns(workouts.GetEnumerator());

            // Mock the FitAppContext and set up the Workouts DbSet
            _mockContext = new Mock<FitAppContext>(new DbContextOptions<FitAppContext>());
            _mockContext.Setup(c => c.Workouts).Returns(mockSet.Object);

            // Initialize the controller with the mock context
            _controller = new WorkoutsController(_mockContext.Object, null);
        }

        /// <summary>
        /// Tests that the Index method returns the correct list of workouts.
        /// </summary>
        [TestMethod]
        public void Index_ReturnsCorrectWorkouts()
        {
            // Arrange
            var workouts = new List<Workouts>
    {
        new Workouts { Id = 1, Name = "Workout A", Description = "Test A" },
        new Workouts { Id = 2, Name = "Workout B", Description = "Test B" }
    }.AsQueryable();

            var mockSet = new Mock<DbSet<Workouts>>();
            mockSet.As<IQueryable<Workouts>>().Setup(m => m.Provider).Returns(workouts.Provider);
            mockSet.As<IQueryable<Workouts>>().Setup(m => m.Expression).Returns(workouts.Expression);
            mockSet.As<IQueryable<Workouts>>().Setup(m => m.ElementType).Returns(workouts.ElementType);
            mockSet.As<IQueryable<Workouts>>().Setup(m => m.GetEnumerator()).Returns(workouts.GetEnumerator());

            _mockContext.Setup(c => c.Workouts).Returns(mockSet.Object);

            // Act
            var result = _controller.Index("", "").Result as ViewResult;
            var model = result.Model as List<Workouts>;

            // Assert
            Assert.IsNotNull(model);
            Assert.AreEqual(2, model.Count);
            Assert.AreEqual("Workout A", model.First().Name);
        }


        /// <summary>
        /// Tests that admin users can access the Create view.
        /// </summary>
        [TestMethod]
        public void Create_AdminUser_CanAccessCreateView()
        {
            // Arrange - Simulate an admin user by setting up a ClaimsPrincipal with the Admin role
            var adminClaim = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
        new Claim(ClaimTypes.Role, "Admin")
            }));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = adminClaim }
            };

            // Act - Call the Create action
            var result = _controller.Create() as ViewResult;

            // Assert - Verify that the result is a ViewResult
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            // Since the view is returned directly, the ViewName is likely null, so no need to check for "Create"
        }


        /// <summary>
        /// Tests that non-admin users cannot access the Create view.
        /// </summary>
        [TestMethod]
        public void Create_NonAdminUser_CannotAccessCreateView()
        {
            // Arrange - Simulate a non-admin user by setting up a ClaimsPrincipal with the User role
            var userClaim = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
        new Claim(ClaimTypes.Role, "User")  // Not an admin role
            }));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = userClaim }
            };

            // Act - Call the Create action
            var result = _controller.Create() as IActionResult;

            // Assert - Verify that non-admin users should not be able to access the Create view
            if (!userClaim.IsInRole("Admin"))
            {
                Assert.IsInstanceOfType(result, typeof(ForbidResult));  // Expect ForbidResult for non-admin users
            }
        }


        /// <summary>
        /// Tests that admin users can access the Delete view.
        /// </summary>
        /// 
        [TestMethod]
        public async Task DeleteWorkout_AdminUser_CanDeleteWorkout()
        {
            // Arrange - Simulate an admin user by setting up a ClaimsPrincipal with the Admin role
            var adminClaim = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
        new Claim(ClaimTypes.Role, "Admin")
            }));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = adminClaim }
            };

            // Mock the workout to be deleted
            var workout = new Workouts { Id = 1, Name = "Test Workout" };

            // Mock the DbSet to return the workout when FindAsync is called
            _mockContext.Setup(c => c.Workouts.FindAsync(1)).ReturnsAsync(workout);

            // Act - Call the DeleteConfirmed action
            var result = await _controller.DeleteConfirmed(1) as RedirectToActionResult;

            // Assert - Verify that the workout was deleted
            _mockContext.Verify(c => c.Workouts.Remove(workout), Times.Once);
            _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<System.Threading.CancellationToken>()), Times.Once);
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName); // Should redirect to Index after successful deletion
        }

        /// <summary>
        /// Tests that non-admin users cannot access the Delete view.
        /// </summary>
        /// 
        [TestMethod]
        public async Task DeleteWorkout_WorkoutNotFound_ReturnsNotFound()
        {
            // Arrange - Simulate an admin user
            var adminClaim = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
        new Claim(ClaimTypes.Role, "Admin")
            }));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = adminClaim }
            };

            // Mock FindAsync to return null, simulating a workout that doesn't exist
            _mockContext.Setup(c => c.Workouts.FindAsync(1)).ReturnsAsync((Workouts)null);

            // Act - Call the DeleteConfirmed action
            var result = await _controller.DeleteConfirmed(1);

            // Assert - Verify that NotFound is returned
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        /// <summary>
        /// Tests that users can save workouts.
        /// </summary>
        /// 
        [TestMethod]
        public async Task SaveWorkout_LoggedInUser_CanSaveWorkoutToProfile()
        {
            // Arrange - Simulate a logged-in user by setting up a ClaimsPrincipal with a user ID
            var user = new IdentityUser { Id = "user1", UserName = "testuser" };
            var userClaim = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
        new Claim(ClaimTypes.NameIdentifier, user.Id)
            }));

            // Set up HttpContext with the ClaimsPrincipal
            var mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.Setup(c => c.User).Returns(userClaim);

            var mockControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext.Object
            };

            // Mock UserManager to return the current user ID
            var mockUserManager = new Mock<UserManager<IdentityUser>>(
                Mock.Of<IUserStore<IdentityUser>>(), null, null, null, null, null, null, null, null);

            // Simulate the GetUserId() method call returning the correct user ID
            mockUserManager.Setup(um => um.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(user.Id);

            // Mock the workout that the user is saving
            var workout = new Workouts { Id = 1, Name = "Yoga", Description = "Relaxing", CaloriesBurned = 100, Duration = new System.TimeSpan(0, 30, 0) };

            // Mock the DbSet for Workouts
            var workouts = new List<Workouts> { workout }.AsQueryable();

            var mockWorkoutsSet = new Mock<DbSet<Workouts>>();
            mockWorkoutsSet.As<IQueryable<Workouts>>().Setup(m => m.Provider).Returns(workouts.Provider);
            mockWorkoutsSet.As<IQueryable<Workouts>>().Setup(m => m.Expression).Returns(workouts.Expression);
            mockWorkoutsSet.As<IQueryable<Workouts>>().Setup(m => m.ElementType).Returns(workouts.ElementType);
            mockWorkoutsSet.As<IQueryable<Workouts>>().Setup(m => m.GetEnumerator()).Returns(workouts.GetEnumerator());

            // Mock FindAsync to return the workout
            mockWorkoutsSet.Setup(m => m.FindAsync(It.IsAny<object[]>())).ReturnsAsync(workout);

            // Mock the DbContext for the UserSpecificWorkouts table
            var mockUserSpecificWorkoutsSet = new Mock<DbSet<UserSpecificWorkout>>();
            _mockContext.Setup(c => c.Workouts).Returns(mockWorkoutsSet.Object);
            _mockContext.Setup(c => c.UserSpecificWorkouts).Returns(mockUserSpecificWorkoutsSet.Object);

            // Create the controller with the mocked UserManager and DbContext
            var controller = new WorkoutsController(_mockContext.Object, mockUserManager.Object)
            {
                ControllerContext = mockControllerContext
            };

            // Act - Call the SaveWorkout method
            var result = await controller.SaveWorkout(1);

            // Assert - Verify that the workout was added to the user's profile
            _mockContext.Verify(c => c.UserSpecificWorkouts.Add(It.Is<UserSpecificWorkout>(uw => uw.UserId == user.Id && uw.Name == "Yoga")), Times.Once);
            _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<System.Threading.CancellationToken>()), Times.Once);

            // Assert the result is a redirect to the Index action
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            var redirectResult = result as RedirectToActionResult;
            Assert.AreEqual("Index", redirectResult.ActionName);
        }

        /// <summary>
        /// Tests that GET Edit return correct workout & 
        /// the controller return not found if does'nt exist.
        /// </summary>
        /// 
        [TestMethod]
        public async Task GetEdit_WorkoutExists_ReturnsViewWithWorkout()
        {
            // Arrange - Simulate an admin user
            var adminClaim = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
        new Claim(ClaimTypes.Role, "Admin")
            }));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = adminClaim }
            };

            // Mock the workout that exists
            var workout = new Workouts { Id = 1, Name = "Yoga", Description = "Relaxing" };

            // Mock FindAsync to return the workout
            _mockContext.Setup(c => c.Workouts.FindAsync(1)).ReturnsAsync(workout);

            // Act - Call the Edit action
            var result = await _controller.Edit(1) as ViewResult;
            var model = result.Model as Workouts;

            // Assert - Verify the correct workout is returned
            Assert.IsNotNull(result);
            Assert.AreEqual(workout, model);
        }

        [TestMethod]
        public async Task GetEdit_WorkoutDoesNotExist_ReturnsNotFound()
        {
            // Arrange - Simulate an admin user
            var adminClaim = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
        new Claim(ClaimTypes.Role, "Admin")
            }));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = adminClaim }
            };

            // Mock FindAsync to return null (workout not found)
            _mockContext.Setup(c => c.Workouts.FindAsync(1)).ReturnsAsync((Workouts)null);

            // Act - Call the Edit action
            var result = await _controller.Edit(1);

            // Assert - Verify NotFound is returned
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }


        /// <summary>
        /// Tests that POST Edit update correctly when valid.
        /// It returns NotFound if the workout ID doesn’t match.
        /// </summary>
        /// 
        [TestMethod]
        public async Task PostEdit_ValidWorkout_UpdatesWorkoutAndRedirects()
        {
            // Arrange - Simulate an admin user
            var adminClaim = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
        new Claim(ClaimTypes.Role, "Admin")
            }));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = adminClaim }
            };

            // Mock the workout that exists and is being updated
            var workout = new Workouts { Id = 1, Name = "Yoga", Description = "Updated Description", Duration = new System.TimeSpan(0, 30, 0), CaloriesBurned = 200 };

            // Mock the DbContext update method
            _mockContext.Setup(c => c.Workouts.FindAsync(1)).ReturnsAsync(workout);
            _mockContext.Setup(c => c.Update(workout));

            // Act - Call the POST Edit action
            var result = await _controller.Edit(1, workout) as RedirectToActionResult;

            // Assert - Verify that the workout was updated and redirected to Index
            _mockContext.Verify(c => c.Update(It.IsAny<Workouts>()), Times.Once);
            _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<System.Threading.CancellationToken>()), Times.Once);
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);
        }

        [TestMethod]
        public async Task PostEdit_WorkoutIdMismatch_ReturnsNotFound()
        {
            // Arrange - Simulate an admin user
            var adminClaim = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
        new Claim(ClaimTypes.Role, "Admin")
            }));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = adminClaim }
            };

            // Mock the workout with a mismatched ID
            var workout = new Workouts { Id = 2, Name = "Yoga", Description = "Updated Description", Duration = new System.TimeSpan(0, 30, 0), CaloriesBurned = 200 };

            // Act - Call the POST Edit action
            var result = await _controller.Edit(1, workout);

            // Assert - Verify that NotFound is returned for ID mismatch
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }
    }
}
