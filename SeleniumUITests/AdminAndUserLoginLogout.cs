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
            _driver.Navigate().GoToUrl("https://localhost:7082/");
            // Delay for 1 seconds
            Thread.Sleep(1000);
        }

        /// <summary>
        /// This test verifies if seeded admin credentials are able to log in
        /// by checking the manage user button, which is reserved for admin privileges only.
        /// </summary>
        [TestMethod]
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
        public void Login_WithValidUserCredentials_ShouldRedirectToHome()
        {
            // Go to login page
            _driver.Navigate().GoToUrl("https://localhost:7082/Identity/Account/Login");

            // Delay for 1 seconds
            Thread.Sleep(1000);

            // Find Id for inputs, then enter user credentials
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
