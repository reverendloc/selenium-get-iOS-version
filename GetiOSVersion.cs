namespace SGiOS.UITests
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
        private RemoteWebDriver driver;
        public TestContext TestContext { get; set; }
        string oldiOSVersion = System.IO.File.ReadAllText(@"oldiOSVersion.txt");

        //You can find the directory to use here by going to chrome://version and copying the profile directory. Using this profile to streamline logging into Apple.
        string googleProfile = "PATHTOPROFILE";

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
            driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(60));

            //Open Apple
            driver.Navigate().GoToUrl(this.appleURL);

            //If you aren't logged in, log in
            if (driver.Title != "Apple Developer")
            {
                driver.FindElementByClassName("ac-gn-account").Click();
                driver.FindElementById("accountname").SendKeys("EMAIL");
                driver.FindElementById("accountpassword").SendKeys("PASSWORD");
                driver.FindElementById("submitButton2").Click();
            }

            //Get the latest iOS Version number
            driver.FindElementByLinkText("Downloads").Click();
            String newiOSVersion = driver.FindElementByXPath("/html/body/main[@id='main']/section[@class='grid'][2]/div[@class='row'][2]/section[@class='table']/ul[@class='table-rows']/li[@class='table-row parent-section'][1]/section[@class='col-40']/ul[@class='download-details']/li[@class='build-number']").Text;

            //TODO:  Can this kick off an appium script or something similar too?
            if (oldiOSVersion != newiOSVersion)
            {
                //Send myself a Slack notification that I need to download a new iOS version
                string urlWithAccessToken = "WEBHOOKS URL WITH ACCESS TOKEN";

                SlackClient client = new SlackClient(urlWithAccessToken);

                client.PostMessage(username: "iOS Version Updater",
                           text: "New iOS Beta Version available!",
                           channel: "@morgan");

                //Update the local version number so next time it runs it should pass unless there another new version. 
                //For some reason this isn't running anymore?
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