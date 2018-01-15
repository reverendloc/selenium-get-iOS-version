namespace Partsunlimited.UITests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using OpenQA.Selenium;
    using OpenQA.Selenium.Chrome;
    using OpenQA.Selenium.Firefox;
    using OpenQA.Selenium.IE;
    using OpenQA.Selenium.Remote;
    using OpenQA.Selenium.PhantomJS;
    using OpenQA.Selenium.Support.UI;

    using System;

        [TestClass]
    public class GetiOSVersion
    {
        private string appleURL = "https://developer.apple.com";
        private string mailURL = "https://mail.google.com";
        private RemoteWebDriver driver;
        private string browser;
        public TestContext TestContext { get; set; }
        string oldiOSVersion = System.IO.File.ReadAllText(@"oldiOSVersion.txt");

        //String leading to your Google Profile. Allows you to use cookies to get around gmail's 2-Factor Auth.
        //To do this, log into gmail on browser, and ensure that you killed all other Chrome windows
        //TODO: Use whatever we use for Bug Reporter instead of GUI email so you can avoid all of this. Can't automate it with Gmail. Can't find XPaths, etc. DOESN'T WORK!

        //You can find the directory to use here by going to chrome://version and copying the profile directory
        string googleProfile = "";

        [TestMethod]
        [TestCategory("Selenium")]
        [Priority(1)]
        [Owner("Chrome")]

        public void VersionLookup()
        {
            System.Diagnostics.Debug.WriteLine("Old version: ", oldiOSVersion);

            //Create Chrome Options to set a user Profile
            ChromeOptions options = new ChromeOptions();
            options.AddArguments("user-data-dir=" + googleProfile );

            //Inititialize browser
            driver = new ChromeDriver(options);
            driver.Manage().Window.Maximize();
            driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(10));

            //Open Apple
            driver.Navigate().GoToUrl(this.appleURL);

            //This block was used before I got profiles working to manually log in to Apple each run
            //TODO: CHECK and IF Download link isn't visible, do the commented out steps to login.
            //driver.FindElementByClassName("ac-gn-account").Click();
            //driver.FindElementById("accountname").SendKeys("APPLE ID");
            //driver.FindElementById("accountpassword").SendKeys("APPLE PASSWORD");
            //driver.FindElementById("submitButton2").Click();

            //Get the latest iOS Version number
            driver.FindElementByLinkText("Downloads").Click();
            String newiOSVersion = driver.FindElementByXPath("/html/body/main[@id='main']/section[@class='grid'][2]/div[@class='row'][2]/section[@class='table']/ul[@class='table-rows']/li[@class='table-row parent-section'][1]/section[@class='col-40']/ul[@class='download-details']/li[@class='build-number']").Text;
            System.Diagnostics.Debug.WriteLine("New version: ", newiOSVersion);

            //IF the iOS version on Apple's page is newer than our local version, we want to email ourselves to test the new version!
            //TODO:  Can this kick off an appium script or something similar too?
            if (oldiOSVersion != newiOSVersion)
            {
                driver.Navigate().GoToUrl(this.mailURL);
                //driver.FindElementByXPath("//*[@id=':4o']/div/div").Click();
                driver.FindElementByCssSelector("#\\3a 4d > div > div").Click();
                //driver.FindElementByXPath("//*[@id=':ne']").Click();

                //driver.FindElementByXPath("//*[@id=':ne']").SendKeys("RECIPIENT EMAIL" + Keys.Tab + Keys.Tab + "iOS Version Out of Date" + Keys.Tab + Keys.Tab + "Test the new iOS Beta!" + Keys.Control + Keys.Enter);
                driver.FindElementByCssSelector("#\\3a n3").SendKeys("RECIPIENT EMAIL" + Keys.Tab + Keys.Tab + "iOS Version Out of Date" + Keys.Tab + Keys.Tab + "Test the new iOS Beta!" + Keys.Control + Keys.Enter);
                //driver.FindElementByName("//*[@id=':mx']").SendKeys("iOS Version Out of Date" + Keys.Tab + Keys.Tab);
                //driver.FindElementByXPath("//*[@id=':nw']").Click();
                //driver.FindElementByXPath("//*[@id=':nw']").SendKeys("Test the new iOS Beta!");
                //driver.FindElementByXPath("//*[@id=':1bt']").Click();

                //Update the local version number so next time it runs it should pass unless there another new version.
                System.IO.File.WriteAllText(@"oldiOSVersion.txt", string.Empty);
                System.IO.File.WriteAllText(@"oldiOSVersion.txt", newiOSVersion);
            }
            Assert.AreEqual(oldiOSVersion, newiOSVersion);


        }

        [TestCleanup()]
    public void MyTestCleanup()
    {
      driver.Quit();
    }

    [TestInitialize()]
    public void MyTestInitialize()
    {
    }
  }
}