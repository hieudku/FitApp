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
            _driver.Navigate().GoToUrl("https://localhost:7082/");
            Thread.Sleep(2000);
        }

        [TestMethod]
        public void aAdmin_ShouldCreateAndVerifyWorkout()
        {
            // Log in as Admin
            LogInAsAdmin();

            // Navigate to Create Workout page
            _driver.Navigate().GoToUrl("https://localhost:7082/Workouts/Create");
            Thread.Sleep(2000);  // Wait for page load

            // Fill out the form to create a workout
            string workoutName = "Selenium Test Workout";
            _driver.FindElement(By.Id("Name")).SendKeys(workoutName);
            _driver.FindElement(By.Id("Description")).SendKeys("Selenium Test workout description.");
            _driver.FindElement(By.Id("Duration")).SendKeys("00:45:00am");
            _driver.FindElement(By.Id("CaloriesBurned")).SendKeys("600");
            _driver.FindElement(By.Id("create_button")).Click();
            Thread.Sleep(2000);  // Wait for form submission

            // Verify workout was created
            // Navigate to Workout page
            _driver.Navigate().GoToUrl("https://localhost:7082/Workouts");

            // Scroll to the newly created workout to ensure it's visible
            ScrollToWorkout(workoutName);
            Thread.Sleep(2000);

            var pageText = _driver.FindElement(By.TagName("body")).Text;
            Assert.IsTrue(pageText.Contains(workoutName), "New workout was not successfully created.");

            // Get the workout ID dynamically from the workout list (assume workout name is unique)

            string workoutId = GetWorkoutIdByName(workoutName);
            Assert.IsNotNull(workoutId, "Could not retrieve workout ID for the created workout.");
        }

        [TestMethod]
        public void bAdmin_ShouldEditWorkout()
        {
            // Log in as Admin
            LogInAsAdmin();

            // Get the workout ID dynamically from the workout list
            string workoutName = "Selenium Test Workout";
            string workoutId = GetWorkoutIdByName(workoutName);
            Assert.IsNotNull(workoutId, "Could not retrieve workout ID for the workout to be edited.");

            // Navigate to Edit Workout page using dynamic ID
            _driver.Navigate().GoToUrl($"https://localhost:7082/Workouts/Edit/{workoutId}");
            Thread.Sleep(2000);  // Wait for page load

            // Edit the workout details
            var descriptionField = _driver.FindElement(By.Id("Description"));
            descriptionField.Clear();
            descriptionField.SendKeys("Updated workout description.");
            _driver.FindElement(By.Id("update_button")).Click();
            Thread.Sleep(2000);  // Wait for form submission

            // Verify workout was edited
            // Scroll to the updated workout
            ScrollToWorkout(workoutName);
            Thread.Sleep(2000);

            var pageText = _driver.FindElement(By.TagName("body")).Text;
            Assert.IsTrue(pageText.Contains("Updated workout description"), "Workout was not successfully updated.");
        }

        [TestMethod]
        public void cAdmin_ShouldDeleteWorkout()
        {
            // Log in as Admin
            LogInAsAdmin();

            // Get the workout ID dynamically from the workout list
            string workoutName = "Selenium Test Workout";
            string workoutId = GetWorkoutIdByName(workoutName);
            Assert.IsNotNull(workoutId, "Could not retrieve workout ID for the workout to be deleted.");

            // Navigate to Delete Workout page using dynamic ID
            _driver.Navigate().GoToUrl($"https://localhost:7082/Workouts/Delete/{workoutId}");
            Thread.Sleep(2000);  // Wait for page load

            // Confirm the deletion
            _driver.FindElement(By.Id("delete_button")).Click();
            Thread.Sleep(2000);  // Wait for form submission

            // Verify workout was deleted
            var pageText = _driver.FindElement(By.TagName("body")).Text;
            Assert.IsFalse(pageText.Contains(workoutName), "Workout was not successfully deleted.");
        }

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
            _driver.Quit();
        }

        private void LogInAsAdmin()
        {
            _driver.Navigate().GoToUrl("https://localhost:7082/Identity/Account/Login");
            Thread.Sleep(2000);  // Wait for login page to load

            _driver.FindElement(By.Id("Input_Email")).SendKeys("admin@fitapp.com");
            _driver.FindElement(By.Id("Input_Password")).SendKeys("Admin123!");
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
