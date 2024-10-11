using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Threading;

namespace CRUDOperationsUITests
{
    [TestClass]
    public class WorkoutCRUDTests
    {
        private static IWebDriver _driver;

        [ClassInitialize]
        public static void Setup(TestContext context)
        {
            _driver = new ChromeDriver();
            _driver.Manage().Window.Maximize();
        }

        [TestInitialize]
        public void TestSetup()
        {
            //_driver.Navigate().GoToUrl("https://localhost:7082/");
            //Thread.Sleep(1000);
        }

        /// <summary>
        /// This test method verify if user can view users workouts 
        /// on their profile by checking if the id for header <h1>My Workouts<h1>
        /// exists in the page view
        /// </summary>
        /// 
        [TestMethod]
        public void aUser_CouldViewUserWorkouts()
        {
            string myWorkoutsHeader = "My Workouts";
            // Log in as admin
            LogInAsUser();

            // Wait for 1 second
            Thread.Sleep(1000);

            // Go to My Workouts link on nav bar
            _driver.FindElement(By.Id("myWorkoutsLink")).Click();

            // Wait for 1 second
            Thread.Sleep(1000);

            var pageText = _driver.FindElement(By.TagName("h1")).Text;
            Assert.IsTrue(pageText.Contains(myWorkoutsHeader), "Could not view My Workouts page.");
        }

        /// <summary>
        /// This test verify if user can successfully add an existing workout
        /// to their profile by searching for workout name added to their user profile.
        /// </summary>
        /// 
        [TestMethod]
        public void bUser_CouldAddWorkouts_ToProfile()
        {
            string workoutName = "Deadlift"; // Pre-existing workout name in db

            // Log in as a regular user
            LogInAsUser();
            Thread.Sleep(1000);

            // Go to the workouts list view
            _driver.Navigate().GoToUrl("https://localhost:7082/Workouts");
            Thread.Sleep(1000);

            // Scroll to the workout by its name
            ScrollToWorkout(workoutName);
            Thread.Sleep(1000);

            // Find the correct workout row by its name
            var workoutRow = _driver.FindElement(By.XPath($"//tr[td[contains(text(), '{workoutName}')]]"));

            // Within the same row, find the associated "Save" button
            var saveButton = workoutRow.FindElement(By.CssSelector("form[action*='/Workouts/SaveWorkout'] button[type='submit']"));

            // Click the "Save" button
            saveButton.Click();
            Thread.Sleep(1000);  // Wait for the action to complete

            // Verify that the workout is now added to "My Workouts"
            _driver.Navigate().GoToUrl("https://localhost:7082/UserSpecificWorkouts/MyWorkouts");
            Thread.Sleep(2000);

            var pageText = _driver.FindElement(By.TagName("body")).Text;
            Assert.IsTrue(pageText.Contains(workoutName), $"Workout '{workoutName}' was not added to the profile.");
        }

        // Cleanup by log out after every test
        [TestCleanup]
        public void TestCleanup()
        {
            var logoutButton = _driver.FindElement(By.Id("Logout_Button"));
            logoutButton.Click();
            Thread.Sleep(2000);  // Wait for logout
        }

        [ClassCleanup]
        public static void TearDown()
        {
            // Thread.Sleep(5000);
            _driver.Quit();
        }

        // Admin log in method
        private void LogInAsAdmin()
        {
            _driver.Navigate().GoToUrl("https://localhost:7082/Identity/Account/Login");
            Thread.Sleep(2000);  // Wait for login page to load

            _driver.FindElement(By.Id("Input_Email")).SendKeys("admin@fitapp.com");
            _driver.FindElement(By.Id("Input_Password")).SendKeys("Admin123!");
            _driver.FindElement(By.CssSelector("button[type='submit']")).Click();
            Thread.Sleep(2000);  // Wait for login to complete
        }

        // User log in method
        private void LogInAsUser()
        {
            _driver.Navigate().GoToUrl("https://localhost:7082/Identity/Account/Login");
            Thread.Sleep(2000);  // Wait for login page to load

            _driver.FindElement(By.Id("Input_Email")).SendKeys("user@fitapp.com");
            _driver.FindElement(By.Id("Input_Password")).SendKeys("User123!");
            _driver.FindElement(By.CssSelector("button[type='submit']")).Click();
            Thread.Sleep(2000);  // Wait for login to complete
        }

        private string GetWorkoutIdByName(string workoutName)
        {
            // Navigate to the workouts list page
            _driver.Navigate().GoToUrl("https://localhost:7082/Workouts");
            Thread.Sleep(2000);  // Wait for page load

            // Find the workout by name and get the associated ID (assumes the name is unique)
            var workoutRow = _driver.FindElement(By.XPath($"//tr[td[contains(text(), '{workoutName}')]]"));
            var workoutIdLink = workoutRow.FindElement(By.CssSelector("a[href*='Workouts/Details/']"));
            var href = workoutIdLink.GetAttribute("href");

            // Extract the workout ID from the URL
            var workoutId = href.Split('/').Last();
            return workoutId;
        }

        private void ScrollToWorkout(string workoutName)
        {
            // Navigate to the workouts list page
            _driver.Navigate().GoToUrl("https://localhost:7082/Workouts");
            Thread.Sleep(2000);  // Wait for page load

            // Find the workout by name
            var workoutRow = _driver.FindElement(By.XPath($"//tr[td[contains(text(), '{workoutName}')]]"));

            // Scroll the workout into view using JavaScript
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", workoutRow);

            // Add a small delay after scrolling to ensure element is visible
            Thread.Sleep(1000);
        }
    }
}
