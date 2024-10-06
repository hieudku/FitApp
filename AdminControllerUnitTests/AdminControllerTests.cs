using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YourNamespace.Tests
{
    /// <summary>
    /// Unit tests for the AdminController class.
    /// These tests cover the main functionalities of the AdminController, including:
    /// - Retrieving a list of users
    /// - Handling user deletion requests
    /// - Check for proper responses for user not found scenarios
    /// - Verifying successful and failed user deletions
    /// </summary>
    /// 
    [TestClass]
    public class AdminControllerTests
    {
        private Mock<UserManager<IdentityUser>> _mockUserManager;
        private AdminController _controller;

        [TestInitialize]
        public void Setup()
        {
            var store = new Mock<IUserStore<IdentityUser>>();
            _mockUserManager = new Mock<UserManager<IdentityUser>>(store.Object, null, null, null, null, null, null, null, null);
            _controller = new AdminController(_mockUserManager.Object);
        }

        /// <summary>
        /// This test verifies that the Users action returns a ViewResult containing a list of users
        /// </summary>
        /// 
        [TestMethod]
        public void Users_ReturnsViewResult_WithListOfUsers()
        {
            // Arrange
            var users = new List<IdentityUser>
            {
                new IdentityUser { UserName = "user1" },
                new IdentityUser { UserName = "user2" }
            }.AsQueryable();
            _mockUserManager.Setup(m => m.Users).Returns(users);

            // Act
            var result = _controller.Users();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = result as ViewResult;
            var model = ((ViewResult)result).ViewData.Model as IEnumerable<IdentityUser>;
            Assert.AreEqual(2, model.Count());
        }

        /// <summary>
        /// This test checks that the DeleteUser action returns a NotFoundResult when the 
        /// user with the specified ID is not found
        /// </summary>
        /// 
        [TestMethod]
        public async Task DeleteUser_UserNotFound_ReturnsNotFound()
        {
            // Arrange
            _mockUserManager.Setup(m => m.FindByIdAsync(It.IsAny<string>())).ReturnsAsync((IdentityUser)null);

            // Act
            var result = await _controller.DeleteUser("1");

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        /// <summary>
        /// This test ensures that the DeleteUser action returns a ViewResult containing the 
        /// user when the user with the specified ID is found
        /// </summary>
        /// 
        [TestMethod]
        public async Task DeleteUser_UserFound_ReturnsViewResult_WithUser()
        {
            // Arrange
            var user = new IdentityUser { Id = "1", UserName = "user1" };
            _mockUserManager.Setup(m => m.FindByIdAsync("1")).ReturnsAsync(user);

            // Act
            var result = await _controller.DeleteUser("1");

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = result as ViewResult;
            var model = ((ViewResult)result).ViewData.Model as IdentityUser;
            Assert.AreEqual("user1", model.UserName);
        }

        /// <summary>
        /// This test verifies that the DeleteUserConfirmed action returns a NotFoundResult 
        /// when the user with the specified ID is not found
        /// </summary>
        /// 
        [TestMethod]
        public async Task DeleteUserConfirmed_UserNotFound_ReturnsNotFound()
        {
            // Arrange
            _mockUserManager.Setup(m => m.FindByIdAsync(It.IsAny<string>())).ReturnsAsync((IdentityUser)null);

            // Act
            var result = await _controller.DeleteUserConfirmed("1");

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        /// <summary>
        /// This test checks that the DeleteUserConfirmed action redirects to the Users 
        /// action when the user with the specified ID is found and successfully deleted
        /// </summary>
        /// 
        [TestMethod]
        public async Task DeleteUserConfirmed_UserFound_DeletionSuccessful_RedirectsToUsers()
        {
            // Arrange
            var user = new IdentityUser { Id = "1", UserName = "user1" };
            _mockUserManager.Setup(m => m.FindByIdAsync("1")).ReturnsAsync(user);
            _mockUserManager.Setup(m => m.DeleteAsync(user)).ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _controller.DeleteUserConfirmed("1");

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            var redirectResult = result as RedirectToActionResult;
            Assert.AreEqual("Users", redirectResult.ActionName);
        }

        /// <summary>
        /// This test ensures that the DeleteUserConfirmed action returns a ViewResult containing the 
        /// user and model state errors when the user with the specified ID is found but deletion fails
        /// </summary>
        /// 
        [TestMethod]
        public async Task DeleteUserConfirmed_UserFound_DeletionFailed_ReturnsViewResult_WithErrors()
        {
            // Arrange
            var user = new IdentityUser { Id = "1", UserName = "user1" };
            var identityErrors = new List<IdentityError> { new IdentityError { Description = "Error" } };
            var identityResult = IdentityResult.Failed(identityErrors.ToArray());
            _mockUserManager.Setup(m => m.FindByIdAsync("1")).ReturnsAsync(user);
            _mockUserManager.Setup(m => m.DeleteAsync(user)).ReturnsAsync(identityResult);

            // Act
            var result = await _controller.DeleteUserConfirmed("1");

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = result as ViewResult;
            var model = ((ViewResult)result).ViewData.Model as IdentityUser;
            Assert.AreEqual("user1", model.UserName);
            Assert.IsTrue(_controller.ModelState.ErrorCount > 0);
        }
    }
}
