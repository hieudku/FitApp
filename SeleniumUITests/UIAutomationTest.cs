using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Threading;

namespace SeleniumUITests
{
    [TestClass]
    public class AppUITests
    {
        private static IWebDriver _driver;

        [ClassInitialize]
        public static void ClassSetup(TestContext context)
        {
            // Initialize the ChromeDriver only once for the whole class
            _driver = new ChromeDriver();
            _driver.Manage().Window.Maximize();
        }

        [TestInitialize]
        public void TestSetup()
        {
            // Navigate to the home page before each test
            // for https
            _driver.Navigate().GoToUrl("https://localhost:7082/");

            // for http
            // _driver.Navigate().GoToUrl("http://localhost:5279/");
            // Delay for 1 seconds
            Thread.Sleep(1000);
        }

        /// <summary>
        /// This test verifies if seeded admin credentials are able to log in
        /// by checking the manage user button, which is reserved for admin privileges only.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        public void Login_WithValidAdminCredentials_ShouldRedirectToHome()
        {
            // Go to login page
            _driver.Navigate().GoToUrl("https://localhost:7082/Identity/Account/Login");

            // Delay for 1 seconds
            Thread.Sleep(1000);

            // Find Id for inputs, then enter admin credentials
            _driver.FindElement(By.Id("Input_Email")).SendKeys("admin@fitapp.com");
            _driver.FindElement(By.Id("Input_Password")).SendKeys("Admin123!");

            // Delay for 1 seconds
            Thread.Sleep(1000);

            // Submit the login form
            _driver.FindElement(By.CssSelector("button[type='submit']")).Click();

            // Delay for 1 seconds
            Thread.Sleep(1000);

            // Assert that login was successful by checking if the Manage User button is accessible.
            var manageUserButton = _driver.FindElement(By.Id("ManageUser_Button"));
            Assert.IsNotNull(manageUserButton, "Login was not successful.");

            // Log out Admin here
            // Delay for 1 seconds
            Thread.Sleep(1000);

            // Find and click the Logout button
            var logoutButton = _driver.FindElement(By.Id("Logout_Button"));
            logoutButton.Click();

            // Assert that the login button is visible after logging out
            var loginButton = _driver.FindElement(By.Id("Login_Button"));
            Assert.IsNotNull(loginButton, "Login button is not displayed after logout.");
        }

        /// <summary>
        /// This test verifies if seeded user credentials are able to log in
        /// by checking if the logout button is accessible by the user
        /// and unable to access manage users button.
        /// </summary>
        [TestMethod]
        [Priority(2)]
        public void Login_WithValidUserCredentials_ShouldRedirectToHome()
        {
            // Go to login page
            _driver.Navigate().GoToUrl("https://localhost:7082/Identity/Account/Login");

            // Delay for 1 seconds
            Thread.Sleep(1000);

            // Find Id for inputs, then enter seeded user credentials
            _driver.FindElement(By.Id("Input_Email")).SendKeys("user@fitapp.com");
            _driver.FindElement(By.Id("Input_Password")).SendKeys("User123!");

            // Delay for 1 seconds
            Thread.Sleep(1000);

            // Submit the login form
            _driver.FindElement(By.CssSelector("button[type='submit']")).Click();

            // Delay for 1 seconds
            Thread.Sleep(1000);

            // Assert that login was successful by checking if the Logout button is accessible.
            var logoutButton = _driver.FindElement(By.Id("Logout_Button"));
            Assert.IsNotNull(logoutButton, "Login was not successful.");

            // Assert that the Manage User button should not be accessible by catching NoSuchElementException
            try
            {
                var manageUserButton = _driver.FindElement(By.Id("ManageUser_Button"));
                Assert.Fail("Manage User button should not be accessible by normal users, but it is visible.");
            }
            catch (NoSuchElementException)
            {
                // Expected outcome: NoSuchElementException means the element is not found, which is correct behavior
                Assert.IsTrue(true, "Manage User button is correctly not accessible by normal users.");
            }

            // Logout user here
            // Delay for 1 seconds
            Thread.Sleep(1000);

            // Find and click the Logout button
            logoutButton.Click();

            // Assert that the login button is visible after logging out
            var loginButton = _driver.FindElement(By.Id("Login_Button"));
            Assert.IsNotNull(loginButton, "Login button is not displayed after logout.");
        }


        /// <summary>
        /// This test check the functionalities of CRUD operations 
        /// performed by admin, and verify admin priviledge of these operations.
        /// </summary>
        [TestMethod]
        [Priority(3)]
        public void Admin_ShouldBeAbleToAccessWorkoutsPage()
        {
            // Navigate to the login page
            _driver.Navigate().GoToUrl("https://localhost:7082/Identity/Account/Login");

            // Delay for 1 seconds
            Thread.Sleep(1000);

            // Log in as Admin
            _driver.FindElement(By.Id("Input_Email")).SendKeys("admin@fitapp.com");
            _driver.FindElement(By.Id("Input_Password")).SendKeys("Admin123!");

            // Submit the login form
            _driver.FindElement(By.CssSelector("button[type='submit']")).Click();

            // Delay for 1 seconds
            Thread.Sleep(1000);

            // Navigate to Workouts page
            _driver.Navigate().GoToUrl("https://localhost:7082/Workouts");

            // Assert the page has loaded by checking for a title or a known element
            var workoutsTitle = _driver.FindElement(By.TagName("h1"));
            Assert.AreEqual("Workouts", workoutsTitle.Text, "Workouts page not accessible to admin.");

            // Delay for 1 seconds
            Thread.Sleep(1000);

            // Find and click the Logout button
            var logoutButton = _driver.FindElement(By.Id("Logout_Button"));
            logoutButton.Click();
        }

        [TestMethod]
        [Priority(4)]
        public void Admin_ShouldBeAbleToCreateWorkout()
        {
            // Delay for 1 seconds
            Thread.Sleep(1000);

            // Navigate to the login page
            _driver.Navigate().GoToUrl("https://localhost:7082/Identity/Account/Login");

            // Delay for 1 seconds
            Thread.Sleep(1000);

            // Log in as Admin
            _driver.FindElement(By.Id("Input_Email")).SendKeys("admin@fitapp.com");
            _driver.FindElement(By.Id("Input_Password")).SendKeys("Admin123!");

            // Delay for 1 seconds
            Thread.Sleep(1000);

            // Submit the login form
            _driver.FindElement(By.CssSelector("button[type='submit']")).Click();

            // Delay for 1 seconds
            Thread.Sleep(1000);

            // Navigate to Workouts page
            _driver.Navigate().GoToUrl("https://localhost:7082/Workouts/Create");

            // Delay for 1 seconds
            Thread.Sleep(1000);

            // Fill in the form fields to create a new workout
            _driver.FindElement(By.Id("Name")).SendKeys("Test Workout");
            _driver.FindElement(By.Id("Description")).SendKeys("A test workout description.");
            _driver.FindElement(By.Id("Duration")).SendKeys("00:30:00am"); // Had to put am at the end because of default html timepicker format
            _driver.FindElement(By.Id("CaloriesBurned")).SendKeys("500");

            // Submit the form
            _driver.FindElement(By.CssSelector("button[type='submit']")).Click();

            // Delay for 1 seconds
            Thread.Sleep(1000);

            // Assert the workout was created by checking if it's listed on the Workouts page
            var workoutsPage = _driver.FindElement(By.TagName("body")).Text;
            Assert.IsTrue(workoutsPage.Contains("Test Workout"), "New workout was not successfully created.");
        }

        [TestMethod]
        [Priority(5)]
        public void Admin_ShouldBeAbleToEditWorkout()
        {
            // Log in as Admin
            _driver.FindElement(By.Id("Input_Email")).SendKeys("admin@fitapp.com");
            _driver.FindElement(By.Id("Input_Password")).SendKeys("Admin123!");

            // Submit the login form
            _driver.FindElement(By.CssSelector("button[type='submit']")).Click();

            // Navigate to Workouts page and click edit on a workout (assuming you know the ID)
            _driver.Navigate().GoToUrl("https://localhost:7082/Workouts/Edit/1");

            // Update the workout details
            var descriptionField = _driver.FindElement(By.Id("Description"));
            descriptionField.Clear();
            descriptionField.SendKeys("Updated workout description.");

            // Submit the form
            _driver.FindElement(By.CssSelector("button[type='submit']")).Click();

            // Assert the workout was updated
            var workoutsPage = _driver.FindElement(By.TagName("body")).Text;
            Assert.IsTrue(workoutsPage.Contains("Updated workout description."), "Workout was not successfully updated.");
        }

        [TestMethod]
        [Priority(6)]
        public void Admin_ShouldBeAbleToDeleteWorkout()
        {
            // Log in as Admin
            _driver.FindElement(By.Id("Input_Email")).SendKeys("admin@fitapp.com");
            _driver.FindElement(By.Id("Input_Password")).SendKeys("Admin123!");

            // Submit the login form
            _driver.FindElement(By.CssSelector("button[type='submit']")).Click();

            // Navigate to Workouts page and click delete on a workout (assuming you know the ID)
            _driver.Navigate().GoToUrl("https://localhost:7082/Workouts/Delete/1");

            // Confirm the delete action
            _driver.FindElement(By.CssSelector("button[type='submit']")).Click();

            // Assert the workout was deleted by checking the page
            var workoutsPage = _driver.FindElement(By.TagName("body")).Text;
            Assert.IsFalse(workoutsPage.Contains("Test Workout"), "Workout was not successfully deleted.");
        }

        [TestMethod]
        [Priority(7)]
        public void User_ShouldNotBeAbleToAccessWorkoutsCreateEditDelete()
        {
            // Log in as a regular user
            _driver.FindElement(By.Id("Input_Email")).SendKeys("user@fitapp.com");
            _driver.FindElement(By.Id("Input_Password")).SendKeys("User123!");

            // Submit the login form
            _driver.FindElement(By.CssSelector("button[type='submit']")).Click();

            // Navigate to Workouts Create page (should fail for regular users)
            _driver.Navigate().GoToUrl("https://localhost:7082/Workouts/Create");

            // Assert that the user is redirected or access is forbidden
            var pageTitle = _driver.FindElement(By.TagName("h1")).Text;
            Assert.AreNotEqual("Create Workout", pageTitle, "Normal user should not have access to create workouts.");
        }

        // Close the browser after test
        [ClassCleanup]
        public static void TearDown()
        {
            // Delay for 1 seconds
            Thread.Sleep(1000);
            _driver.Quit();  
        }
        
    }
}
