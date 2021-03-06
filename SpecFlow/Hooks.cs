using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using System.Reflection;
using AventStack.ExtentReports.Gherkin.Model;
using OpenQA.Selenium.Remote;
using System.IO;
using AventStack.ExtentReports.Reporter.Configuration;
using SpecFlow.Library;
using OpenQA.Selenium.Firefox;
using BoDi;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Configuration;

namespace SpecFlow
{
    [Binding]
    public class Hooks
    {
        private readonly IObjectContainer objectContainer;
        private RemoteWebDriver driver;
        private static ExtentTest featureName;
        private static ExtentTest scenario;
        private static ExtentReports extent;

        public Hooks(IObjectContainer objectContainer)
        {
            this.objectContainer = objectContainer;
        }

        [BeforeTestRun]
        public static void BeforeTestRun()
        {
            //Initialize Extent report before test starts
            var htmlReporter = new ExtentHtmlReporter(Directory.GetCurrentDirectory() + @"\Reports\ExtentReport.html");
            htmlReporter.Configuration().Theme = Theme.Standard;
            //Attach report to reporter
            extent = new ExtentReports();
            extent.AttachReporter(htmlReporter);
        }

        [AfterTestRun]
        public static void AfterTestRun()
        {
            //Flush report once test completes
            extent.Flush();
        }

        [BeforeFeature]
        public static void BeforeFeature()
        {
            featureName = extent.CreateTest<AventStack.ExtentReports.Gherkin.Model.Feature>(FeatureContext.Current.FeatureInfo.Title);
        }

        [BeforeScenario]
        public void BeforeScenario()
        {
            scenario = featureName.CreateNode<Scenario>(ScenarioContext.Current.ScenarioInfo.Title);
            SelectBrowser(BrowserType.Firefox);
            NavigateToURL(new Uri(ConfigurationManager.AppSettings["URL"].ToString()));
        }

        [AfterScenario]
        public void AfterScenario()
        {
            driver.Quit();
        }

        [AfterStep]
        public void AfterStep()
        {

            var stepType = ScenarioStepContext.Current.StepInfo.StepDefinitionType.ToString();

            PropertyInfo pInfo = typeof(ScenarioContext).GetProperty("ScenarioExecutionStatus", BindingFlags.Instance | BindingFlags.Public);
            MethodInfo getter = pInfo.GetGetMethod(nonPublic: true);
            object TestResult = getter.Invoke(ScenarioContext.Current, null);

            if (ScenarioContext.Current.TestError == null)
            {
                if (stepType == "Given")
                    scenario.CreateNode<Given>(ScenarioStepContext.Current.StepInfo.Text);
                else if (stepType == "When")
                    scenario.CreateNode<When>(ScenarioStepContext.Current.StepInfo.Text);
                else if (stepType == "Then")
                    scenario.CreateNode<Then>(ScenarioStepContext.Current.StepInfo.Text);
                else if (stepType == "And")
                    scenario.CreateNode<And>(ScenarioStepContext.Current.StepInfo.Text);
            }
            else if (ScenarioContext.Current.TestError != null)
            {
                if (stepType == "Given")
                    scenario.CreateNode<Given>(ScenarioStepContext.Current.StepInfo.Text).Fail(ScenarioContext.Current.TestError.InnerException);
                else if (stepType == "When")
                    scenario.CreateNode<When>(ScenarioStepContext.Current.StepInfo.Text).Fail(ScenarioContext.Current.TestError.InnerException);
                else if (stepType == "Then")
                    scenario.CreateNode<Then>(ScenarioStepContext.Current.StepInfo.Text).Fail(ScenarioContext.Current.TestError.Message);
            }

            //Pending Status
            if (TestResult.ToString() == "StepDefinitionPending")
            {
                if (stepType == "Given")
                    scenario.CreateNode<Given>(ScenarioStepContext.Current.StepInfo.Text).Skip("Step Definition Pending");
                else if (stepType == "When")
                    scenario.CreateNode<When>(ScenarioStepContext.Current.StepInfo.Text).Skip("Step Definition Pending");
                else if (stepType == "Then")
                    scenario.CreateNode<Then>(ScenarioStepContext.Current.StepInfo.Text).Skip("Step Definition Pending");

            }

        }

        internal void SelectBrowser(BrowserType browserType)
        {
            switch (browserType)
            {
                case BrowserType.Chrome:
                    var cdriverDir = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\Resource\";
                    ChromeDriverService cservice = ChromeDriverService.CreateDefaultService(cdriverDir, "chromedriver.exe");
                    cservice.HideCommandPromptWindow = true;
                    cservice.SuppressInitialDiagnosticInformation = true;
                    //ChromeOptions option = new ChromeOptions();
                    //option.AddArgument("--headless");
                    driver = new ChromeDriver(cservice);
                    objectContainer.RegisterInstanceAs<IWebDriver>(driver);
                    break;

                case BrowserType.Firefox:
                    var fdriverDir = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\Resource\";
                    FirefoxDriverService fservice = FirefoxDriverService.CreateDefaultService(fdriverDir, "geckodriver.exe");
                    //service.FirefoxBinaryPath = @"C:\Program Files (x86)\Mozilla Firefox\firefox.exe";
                    fservice.HideCommandPromptWindow = true;
                    fservice.SuppressInitialDiagnosticInformation = true;
                    driver = new FirefoxDriver(fservice);
                    objectContainer.RegisterInstanceAs<RemoteWebDriver>(driver);
                    break;

                case BrowserType.IE:
                    break;

                default:
                    break;
            }
        }

        internal void NavigateToURL(Uri uri)
        {
            driver.Navigate().GoToUrl(uri);
        }
    }

    enum BrowserType
    {
        Chrome,
        Firefox,
        IE
    }
}
